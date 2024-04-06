using System.ComponentModel;

namespace ASA_Server_Manager.Interfaces.ViewModels;

public interface IViewModel : INotifyPropertyChanged, IDisposable
{
    object View { get; set; }

    void OnViewLoaded();

    void OnViewUnloaded();
}