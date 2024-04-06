namespace ASA_Server_Manager.Interfaces.Helpers;

public interface IProcessMonitor : IDisposable
{
    event EventHandler<bool> IsRunningChanged;

    bool IsRunning { get; }

    void Start(TimeSpan? timeSpan = null);

    void Stop();
}