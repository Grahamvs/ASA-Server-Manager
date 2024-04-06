using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using ASA_Server_Manager.Interfaces.ViewModels;
using ASA_Server_Manager.Interfaces.Views;

namespace ASA_Server_Manager.Views;

public abstract class BaseWindow : Window
{
    /// <summary> Returns <see langword="true" /> when <see cref="ShowDialog" /> was called, <see langword="false" /> when <see cref="Show" /> was called, and <see langword="null" /> if neither methods were called or if were called as their base class <see cref="Window" />. </summary>
    public bool? IsDialog { get; private set; }

    public new void Show()
    {
        IsDialog = false;
        base.Show();
    }

    public new bool? ShowDialog()
    {
        IsDialog = true;
        return base.ShowDialog();
    }

    protected void CloseWindow(bool? result)
    {
        if (IsDialog == true)
        {
            DialogResult = result;
        }
        else
        {
            Close();
        }
    }
}

public class BaseWindow<TViewModel> : BaseWindow, IView<TViewModel>
    where TViewModel : class, IViewModel
{
    #region Private Fields

    private TViewModel _viewModel;
    private IDisposable _viewModelSubs;

    #endregion

    #region Public Constructors

    public BaseWindow()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        Closing += OnClosing;
    }

    #endregion

    #region Public Properties

    public TViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            DataContext = _viewModel;

            if (IsLoaded)
            {
                ViewModelChanged();
            }
        }
    }

    #endregion

    #region Protected Methods

    protected static void DisposeField<T>(ref T disposable)
        where T : IDisposable
    {
        disposable?.Dispose();
        disposable = default;
    }

    protected void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModelChanged();

        ViewModel?.OnViewLoaded();
    }

    protected void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel?.OnViewUnloaded();

        DisposeField(ref _viewModelSubs);
    }

    protected virtual void OnViewModelChanged()
    {
    }

    #endregion

    #region Private Methods

    private void OnClosing(object sender, CancelEventArgs e)
    {
        if (ViewModel is not IWindowViewModel wvm)
        {
            return;
        }

        if (!wvm.OnWindowClosing())
        {
            e.Cancel = true;
        }
    }

    private void ViewModelChanged()
    {
        DisposeField(ref _viewModelSubs);

        if (ViewModel is IWindowViewModel wvm)
        {
            _viewModelSubs = Observable.FromEventPattern<bool?>(
                    h => wvm.CloseRequested += h,
                    h => wvm.CloseRequested -= h
                )
                .Subscribe(pattern => CloseWindow(pattern.EventArgs));
        }

        OnViewModelChanged();
    }

    #endregion
}