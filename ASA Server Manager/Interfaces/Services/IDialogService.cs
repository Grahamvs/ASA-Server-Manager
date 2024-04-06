using System.Windows;

namespace ASA_Server_Manager.Interfaces.Services;

public interface IDialogService
{
    #region Public Methods

    string OpenFileDialog(string filter, string initialDirectory = null, string fileName = null, string title = null);

    string OpenFolderDialog(string initialDirectory = null, string title = null);

    string SaveFileDialog(string defaultExt, string filter, string fileName = null, string title = null);

    MessageBoxResult ShowErrorMessage(
        string message,
        string caption = null,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxResult defaultResponse = MessageBoxResult.OK
    );

    MessageBoxResult ShowMessage(
        string message,
        string caption = null,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None,
        MessageBoxResult defaultResponse = MessageBoxResult.OK
    );

    MessageBoxResult ShowWarningMessage(
        string message,
        string caption = null,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxResult defaultResponse = MessageBoxResult.OK
    );

    #endregion
}