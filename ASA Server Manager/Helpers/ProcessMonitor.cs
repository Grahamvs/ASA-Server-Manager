using System.Diagnostics;
using ASA_Server_Manager.Interfaces.Helpers;

namespace ASA_Server_Manager.Helpers;

public class ProcessMonitor : IProcessMonitor
{
    private readonly string _processName;
    private readonly TimeSpan _timeSpan;
    private Timer _timer;
    private bool _disposed;
    private bool _isRunning;

    public event EventHandler<bool> IsRunningChanged;

    public ProcessMonitor(string processName, TimeSpan? timeSpan = null)
    {
        _processName = processName;
        _timeSpan = timeSpan ?? TimeSpan.FromSeconds(1);
    }

    public void Start(TimeSpan? timeSpan = null)
    {
        _timer?.Dispose();
        _timer = new Timer(CheckProcessStatus, null, TimeSpan.Zero, timeSpan ?? _timeSpan);
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    private void CheckProcessStatus(object state) => IsRunning = Process.GetProcessesByName(_processName).Any();

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (_isRunning != value)
            {
                _isRunning = value;
                IsRunningChanged?.Invoke(this, _isRunning);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        Stop();
    }
}