using System.Windows.Input;

namespace ASA_Server_Manager.Interfaces.Common.Commands;

public interface ICommand<T> : ICommand
{
    bool CanExecute(T parameter);

    void Execute(T parameter);
}