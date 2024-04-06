using ASA_Server_Manager.Interfaces.ViewModels;

namespace ASA_Server_Manager.ViewModels;

public abstract class WindowViewModel : ViewModel, IWindowViewModel
{
    #region Public Events

    public event EventHandler<bool?> CloseRequested;

    #endregion

    #region Public Methods

    public virtual bool OnWindowClosing()
    {
        return true;
    }

    #endregion

    #region Protected Methods

    protected void RaiseCloseRequested(bool? dialogResult = null) => CloseRequested?.Invoke(this, dialogResult);

    #endregion
}