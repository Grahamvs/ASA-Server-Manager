using ASA_Server_Manager.Interfaces.ViewModels;

namespace ASA_Server_Manager.Interfaces.Views;

public interface IView<TViewModel>
    where TViewModel : IViewModel
{
    TViewModel ViewModel { get; set; }
}