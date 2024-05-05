namespace ASA_Server_Manager.Interfaces.Services;

public interface IToastService : IDisposable
{
    void ShowError(string message, Action onClickAction = null, Action onCloseClickedAction = null);

    void ShowInformation(string message, Action onClickAction = null, Action onCloseClickedAction = null);

    void ShowSuccess(string message, Action onClickAction = null, Action onCloseClickedAction = null);

    void ShowWarning(string message, Action onClickAction = null, Action onCloseClickedAction = null);
}