using System.Windows;
using ASA_Server_Manager.Interfaces.ViewModels;
using ASA_Server_Manager.Interfaces.Views;

namespace ASA_Server_Manager.Interfaces.Services;

public interface IViewService
{
    Window CreateWindow<TViewModel>(TViewModel viewModel = null)
        where TViewModel : class, IViewModel;

    IView<TViewModel> GetView<TViewModel>(TViewModel viewModel = null)
        where TViewModel : class, IViewModel;

    TViewModel GetViewModel<TViewModel>()
        where TViewModel : class, IViewModel;

    Window GetWindow<TViewModel>(TViewModel viewModel)
        where TViewModel : class, IViewModel;

    void ShowView<TViewModel>(TViewModel viewModel = null, WindowStartupLocation? startupLocation = null, object owner = null)
        where TViewModel : class, IViewModel;

    bool? ShowViewDialog<TViewModel>(TViewModel viewModel = null, WindowStartupLocation? startupLocation = null, object owner = null)
        where TViewModel : class, IViewModel;
}