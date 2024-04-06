using System.Linq.Expressions;
using System.Windows.Input;

namespace ASA_Server_Manager.Common.Commands;

public abstract class ActionCommandBase<TCommand, TParameter> : ICommand
    where TCommand : class
{
    #region Private Fields

    private readonly HashSet<string> _observedPropertiesExpressions = [];
    private bool _disposed;
    private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

    #endregion

    #region Public Events

    public event EventHandler CanExecuteChanged;

    #endregion

    #region Public Methods

    bool ICommand.CanExecute(object parameter) => OnCanExecute(Convert(parameter));

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        OnDispose();

        _synchronizationContext = null;
    }

    void ICommand.Execute(object parameter)
    {
        if (parameter is CommandParameter {IsCancelled: true})
            return;

        OnExecute(Convert(parameter));
    }

    public void RaiseCanExecuteChanged()
    {
        if (_synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current)
        {
            _synchronizationContext.Post(_ => RaiseAction(), null);
        }
        else
        {
            RaiseAction();
        }

        return;

        void RaiseAction() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
    /// </summary>
    /// <typeparam name="TType"> The object type containing the property specified in the expression. </typeparam>
    /// <param name="propertyExpression"> The property expression. Example: ObservesProperty(() =&gt; PropertyName). </param>
    public TCommand ObservesProperty<TType>(Expression<Func<TType>> propertyExpression)
    {
        if (!_observedPropertiesExpressions.Contains(propertyExpression.ToString()))
        {
            _observedPropertiesExpressions.Add(propertyExpression.ToString());
            PropertyObserver.Observes(propertyExpression, RaiseCanExecuteChanged);
        }

        return this as TCommand;
    }

    #endregion

    #region Protected Methods

    private TParameter Convert(object parameter) => parameter is TParameter param ? param : default;

    protected abstract bool OnCanExecute(TParameter parameter);

    protected abstract void OnDispose();

    protected abstract void OnExecute(TParameter parameter);

    #endregion
}