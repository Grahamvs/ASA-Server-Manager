using ASA_Server_Manager.Helpers;
using ASA_Server_Manager.Interfaces.Helpers;
using ASA_Server_Manager.Interfaces.Serialization;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Interfaces.ViewModels;
using ASA_Server_Manager.Interfaces.Views;
using ASA_Server_Manager.Serialization;
using ASA_Server_Manager.Services;
using ASA_Server_Manager.ViewModels;
using ASA_Server_Manager.Views;
using LightInject;

namespace ASA_Server_Manager;

public static class DIModule
{
    private static readonly Lazy<IServiceContainer> AppContainer = new(() => GetNewContainer(true));

    public static IServiceContainer GetAppContainer() => AppContainer.Value;

    public static IServiceContainer GetNewContainer(bool includeRegistrations)
    {
        var container = new ServiceContainer();

        // Register self
        container.RegisterInstance<IServiceContainer>(container);

        if (includeRegistrations)
        {
            SetRegistrations(container);
        }

        return container;
    }

    private static void SetRegistrations(IServiceRegistry container)
    {
        // Views & ViewModels
        RegisterViewAndModel<IMainViewModel, MainViewModel, MainWindow>();
        RegisterViewAndModel<ISettingsViewModel, SettingsViewModel, SettingsWindow>();
        RegisterViewAndModel<IAvailableModsViewModel, AvailableModsViewModel, AvailableModsWindow>();
        RegisterViewAndModel<IAboutViewModel, AboutViewModel, AboutWindow>();

        // Services & Helpers
        container.Register<IViewService, ViewService>();
        container.Register<IDialogService, DialogService>();
        container.Register<ISerializer, JsonSerializer>();
        container.Register<IServerHelper, ServerHelper>();
        container.Register<IProcessHelper, ProcessHelper>();
        container.Register<IDownloadHelper, DownloadHelper>();

        // Services (Singletons)
        container.RegisterSingleton<IAppSettingsService, AppSettingsService>();
        container.RegisterSingleton<IFileSystemService, FileSystemService>();
        container.RegisterSingleton<IServerProfileService, ServerProfileService>();
        container.RegisterSingleton<IMapService, MapService>();
        container.RegisterSingleton<IModService, ModService>();
        container.RegisterSingleton<IApplicationService, ApplicationService>();

        return;

        void RegisterViewAndModel<TViewModel, TViewModelImp, TView>()
            where TViewModel : IViewModel
            where TViewModelImp : TViewModel
            where TView : IView<TViewModel> =>
            container
                .Register<TViewModel, TViewModelImp>()
                .Register<IView<TViewModel>, TView>();
    }
}