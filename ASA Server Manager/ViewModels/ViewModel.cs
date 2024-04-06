using ASA_Server_Manager.Common;
using ASA_Server_Manager.Interfaces.ViewModels;

namespace ASA_Server_Manager.ViewModels;

public abstract class ViewModel : BindableBase, IViewModel
{
    #region Private Fields

    private bool _disposed;
    private bool _viewLoaded;

    #endregion

    #region Public Properties

    object IViewModel.View { get; set; }

    #endregion

    #region Protected Properties

    protected bool ViewLoaded => _viewLoaded;

    #endregion

    #region Public Methods

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_disposed)
            return;

        _disposed = true;
        OnDisposed();
    }

    public void OnViewLoaded()
    {
        OnLoad();

        _viewLoaded = true;
    }

    public void OnViewUnloaded()
    {
        _viewLoaded = false;

        OnUnload();
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// This method will only be called once, regardless of how many times <see cref="Dispose" /> is called.
    /// </summary>
    protected virtual void OnDisposed()
    {
    }

    protected abstract void OnLoad();

    protected abstract void OnUnload();

    #endregion
}