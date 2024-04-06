using System.Windows;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Interfaces.ViewModels;
using ASA_Server_Manager.Interfaces.Views;
using ASA_Server_Manager.Views;
using LightInject;

namespace ASA_Server_Manager.Services;

public class ViewService : IViewService
{
    #region Private Fields

    private readonly IServiceContainer _container;

    #endregion

    #region Public Constructors

    public ViewService(IServiceContainer container)
    {
        _container = container;
    }

    #endregion

    #region Public Methods

    public Window CreateWindow<TViewModel>(TViewModel viewModel = null)
        where TViewModel : class, IViewModel
    {
        var view = (viewModel?.View ?? GetView(viewModel)) as IView<TViewModel>;

        if (GetWindow(view?.ViewModel) is not { } window)
        {
            window = new BaseWindow<TViewModel>
            {
                ViewModel = viewModel,
                Content = view,
            };
        }

        return window;
    }

    public IView<TViewModel> GetView<TViewModel>(TViewModel viewModel = null)
        where TViewModel : class, IViewModel
    {
        // Resolve the View from the container
        var view = _container.GetInstance<IView<TViewModel>>();

        // Set the DataContext of the View to the ViewModel. Resolve the ViewModel from the
        // container if needed.
        var vm = viewModel ?? GetViewModel<TViewModel>();
        view.ViewModel = vm;

        vm.View = view;

        return view;
    }

    public TViewModel GetViewModel<TViewModel>()
        where TViewModel : class, IViewModel =>
        _container.GetInstance<TViewModel>();

    public Window GetWindow<TViewModel>(TViewModel viewModel)
        where TViewModel : class, IViewModel =>
        FindWindow(viewModel?.View as DependencyObject);

    public void ShowView<TViewModel>(TViewModel viewModel = null, WindowStartupLocation? startupLocation = null, object owner = null)
        where TViewModel : class, IViewModel =>
        ShowViewInternal(false, viewModel, startupLocation, owner);

    public bool? ShowViewDialog<TViewModel>(TViewModel viewModel = null, WindowStartupLocation? startupLocation = null, object owner = null)
        where TViewModel : class, IViewModel =>
        ShowViewInternal(true, viewModel, startupLocation, owner);

    #endregion

    #region Private Methods

    private Window FindWindow(DependencyObject depObject) =>
        depObject switch
        {
            null => null,
            Window window => window,
            _ => Window.GetWindow(depObject)
        };

    private bool? ShowViewInternal<TViewModel>(bool isDialog, TViewModel viewModel, WindowStartupLocation? startupLocation, object owner)
        where TViewModel : class, IViewModel
    {
        var view = GetView(viewModel);

        // Check if the view is a window
        if (view is not Window window)
        {
            window = new BaseWindow<TViewModel>
            {
                ViewModel = viewModel,
                Content = view,
            };
        }

        if (startupLocation != null)
        {
            window.WindowStartupLocation = startupLocation.Value;
        }

        if (owner != null)
        {
            var ownerView = owner is IViewModel vm
                ? vm.View
                : owner;

            window.Owner = FindWindow(ownerView as DependencyObject);
        }

        // Show the window
        switch (window)
        {
            case BaseWindow baseWindow when isDialog:
                return baseWindow.ShowDialog();

            case BaseWindow baseWindow:
                baseWindow.Show();
                return null;

            case not null when isDialog:
                return window.ShowDialog();

            default:
                window.Show();
                return null;
        }
    }

    #endregion
}