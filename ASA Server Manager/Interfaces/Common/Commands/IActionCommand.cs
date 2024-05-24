using System.Linq.Expressions;

namespace ASA_Server_Manager.Interfaces.Common.Commands;

public interface IActionCommand : IActionCommandBase
{
    bool CanExecute();

    void Execute();

    IActionCommand ObservesProperty<TType>(Expression<Func<TType>> propertyExpression);
}

public interface IActionCommand<T> : ICommand<T>
{
    IActionCommand<T> ObservesProperty<TType>(Expression<Func<TType>> propertyExpression);
}