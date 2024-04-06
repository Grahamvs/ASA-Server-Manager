namespace ASA_Server_Manager.Common;

public class TokenHelper
{
    private readonly List<DisposeAction> _tokens = [];

    public event EventHandler HasTokensChanged;

    private Action<bool> _onChangedAction;

    public TokenHelper(Action<bool> onChangedAction = null)
    {
        _onChangedAction = onChangedAction;
    }

    public bool HasTokens => _tokens.Count > 0;

    public void Clear() => _tokens.ToList().ForEach(token => token.Dispose());

    public IDisposable GetToken()
    {
        var token = new DisposeAction();
        token.Action = () => RemoveToken(token);

        _tokens.Add(token);

        if (_tokens.Count == 1)
        {
            _onChangedAction(true);
            RaiseHasTokensChanged();
        }

        return token;
    }

    private void RemoveToken(DisposeAction token)
    {
        if (_tokens.Remove(token) && _tokens.Count == 0)
        {
            _onChangedAction(false);
            RaiseHasTokensChanged();
        }
    }

    private void RaiseHasTokensChanged() => HasTokensChanged?.Invoke(this, EventArgs.Empty);
}