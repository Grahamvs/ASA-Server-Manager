using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace ASA_Server_Manager.Common;

internal class PropertyObserver
{
    private readonly Action _action;

    private PropertyObserver(Expression propertyExpression, Action action)
    {
        _action = action;
        SubscribeListeners(propertyExpression);
    }

    private void SubscribeListeners(Expression propertyExpression)
    {
        var propNameStack = new Stack<PropertyInfo>();
        while (propertyExpression is MemberExpression temp) // Gets the root of the property chain.
        {
            propertyExpression = temp.Expression;

            if (temp.Member is PropertyInfo propertyInfo)
            {
                propNameStack.Push(propertyInfo); // Records the member info as property info
            }
        }

        if (propertyExpression is not ConstantExpression constantExpression)
            throw new NotSupportedException("Operation not supported for the given expression type. Only MemberExpression and ConstantExpression are currently supported.");

        var propObserverNodeRoot = new PropertyObserverNode(propNameStack.Pop(), _action);
        PropertyObserverNode previousNode = propObserverNodeRoot;
        foreach (var propName in propNameStack) // Create a node chain that corresponds to the property chain.
        {
            var currentNode = new PropertyObserverNode(propName, _action);
            previousNode.Next = currentNode;
            previousNode = currentNode;
        }

        object propOwnerObject = constantExpression.Value;

        if (propOwnerObject is not INotifyPropertyChanged inpcObject)
            throw new InvalidOperationException($"Trying to subscribe PropertyChanged listener in object that owns '{propObserverNodeRoot.PropertyInfo.Name}' property, but the object does not implements INotifyPropertyChanged.");

        propObserverNodeRoot.SubscribeListenerFor(inpcObject);
    }

    /// <summary>
    /// Observes a property that implements INotifyPropertyChanged, and automatically calls a custom
    /// action on property changed notifications. The given expression must be in this form: "()
    /// =&gt; Prop.NestedProp.PropToObserve".
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression representing property to be observed. Ex.: "() =&gt; Prop.NestedProp.PropToObserve".
    /// </param>
    /// <param name="action"> Action to be invoked when PropertyChanged event occurs. </param>
    internal static PropertyObserver Observes<T>(Expression<Func<T>> propertyExpression, Action action)
    {
        return new PropertyObserver(propertyExpression.Body, action);
    }

    internal class PropertyObserverNode
    {
        private readonly Action _action;
        private INotifyPropertyChanged _inpcObject;

        public PropertyInfo PropertyInfo { get; }

        public PropertyObserverNode Next { get; set; }

        public PropertyObserverNode(PropertyInfo propertyInfo, Action action)
        {
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            _action = () =>
            {
                action?.Invoke();
                if (Next == null) return;
                Next.UnsubscribeListener();
                GenerateNextNode();
            };
        }

        public void SubscribeListenerFor(INotifyPropertyChanged inpcObject)
        {
            _inpcObject = inpcObject;
            _inpcObject.PropertyChanged += OnPropertyChanged;

            if (Next != null) GenerateNextNode();
        }

        private void GenerateNextNode()
        {
            var nextProperty = PropertyInfo.GetValue(_inpcObject);
            if (nextProperty == null) return;
            if (nextProperty is not INotifyPropertyChanged nextInpcObject)
                throw new InvalidOperationException($"Trying to subscribe PropertyChanged listener in object that owns '{Next?.PropertyInfo.Name}' property, but the object does not implements INotifyPropertyChanged.");

            Next?.SubscribeListenerFor(nextInpcObject);
        }

        private void UnsubscribeListener()
        {
            if (_inpcObject != null)
                _inpcObject.PropertyChanged -= OnPropertyChanged;

            Next?.UnsubscribeListener();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e?.PropertyName == PropertyInfo.Name || string.IsNullOrEmpty(e?.PropertyName))
            {
                _action?.Invoke();
            }
        }
    }
}