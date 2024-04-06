using System.Windows.Input;

namespace ASA_Server_Manager.Interfaces.Common.Commands;

public interface IActionCommandBase : ICommand, IDisposable
{
    void RaiseCanExecuteChanged();
}