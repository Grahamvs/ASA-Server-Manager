using System.Linq.Expressions;
using ASA_Server_Manager.Interfaces.Common.Commands;

namespace ASA_Server_Manager.Common.Commands;

public class ActionCommand : ActionCommandBase<ActionCommand, object>, IActionCommand
{
    #region Private Fields

    private Func<bool> _canExecute;
    private Action _executeAction;

    #endregion

    #region Public Constructors

    public ActionCommand(Action executeAction, Func<bool> canExecute = null)
    {
        _executeAction = executeAction;
        _canExecute = canExecute;
    }

    #endregion

    #region Public Methods

    public bool CanExecute() => _canExecute?.Invoke() ?? true;

    public void Execute()
    {
        _executeAction.Invoke();
    }

    IActionCommand IActionCommand.ObservesProperty<TType>(Expression<Func<TType>> propertyExpression) => ObservesProperty(propertyExpression);

    #endregion

    #region Protected Methods

    protected override bool OnCanExecute(object parameter) => CanExecute();

    protected override void OnDispose()
    {
        _canExecute = null;
        _executeAction = null;
    }

    protected override void OnExecute(object parameter) => Execute();

    #endregion
}

public class ActionCommand<T> : ActionCommandBase<ActionCommand<T>, T>, IActionCommand<T>
{
    #region Private Fields

    private Predicate<T> _canExecute;
    private Action<T> _executeAction;

    #endregion

    #region Public Constructors

    public ActionCommand(Action<T> executeAction, Predicate<T> canExecute = null)
    {
        _executeAction = executeAction;
        _canExecute = canExecute;
    }

    #endregion

    #region Public Methods

    public bool CanExecute(T parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(T parameter)
    {
        if (parameter is CommandParameter {IsCancelled: true})
            return;

        _executeAction.Invoke(parameter);
    }

    IActionCommand<T> IActionCommand<T>.ObservesProperty<TType>(Expression<Func<TType>> propertyExpression) => ObservesProperty(propertyExpression);

    #endregion

    #region Protected Methods

    protected override bool OnCanExecute(T parameter) => CanExecute(parameter);

    protected override void OnDispose()
    {
        _executeAction = null;
        _canExecute = null;
    }

    protected override void OnExecute(T parameter) => Execute(parameter);

    #endregion
}