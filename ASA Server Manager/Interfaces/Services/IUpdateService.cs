namespace ASA_Server_Manager.Interfaces.Services;

public interface IUpdateService
{
    Task CheckForUpdates(bool showNoUpdate, bool overrideIgnore, IToastService toastService = null);

    void Start();

    void Stop();
}