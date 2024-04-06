using ASA_Server_Manager.Interfaces.Configs;

namespace ASA_Server_Manager.Interfaces.ViewModels;

public interface IMainViewModel : IWindowViewModel
{
    IServerProfile CurrentProfile { get; }
}