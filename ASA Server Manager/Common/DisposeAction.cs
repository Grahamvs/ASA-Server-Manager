namespace ASA_Server_Manager.Common;

public class DisposeAction : IDisposable
{
    private Action _onDispose;

    public DisposeAction(Action onDispose = null)
    {
        _onDispose = onDispose;
    }

    public Action Action
    {
        get => _onDispose;
        set => _onDispose = value;
    }

    public void Dispose()
    {
        Action?.Invoke();
    }
}