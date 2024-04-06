using System.Reflection;
using System.Windows;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Interfaces.ViewModels;
using LightInject;

namespace ASA_Server_Manager;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            SetDropDownMenuToBeRightAligned();

            using var container = DIModule.GetAppContainer();

            var appSettingsService = container.GetInstance<IAppSettingsService>();
            appSettingsService.LoadSettings();

            var viewService = container.GetInstance<IViewService>();

            if (appSettingsService.ServerPath.IsNullOrWhiteSpace())
            {
                viewService.ShowViewDialog<ISettingsViewModel>(startupLocation: WindowStartupLocation.CenterScreen);
            }

            if (!appSettingsService.ServerPath.IsNullOrWhiteSpace())
            {
                container.GetInstance<IMapService>().RefreshAvailableMaps();
                container.GetInstance<IServerProfileService>().LoadLastProfile();
                container.GetInstance<IModService>().Load();

                Current.MainWindow = viewService.CreateWindow<IMainViewModel>();

                Current.MainWindow?.ShowDialog();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);

            MessageBox.Show(
                exception.Message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error,
                MessageBoxResult.OK
            );
        }

        Current.Shutdown();
    }

    private static void SetDropDownMenuToBeRightAligned()
    {
        var menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);

        void SetAlignmentValue()
        {
            if (SystemParameters.MenuDropAlignment && menuDropAlignmentField != null) menuDropAlignmentField.SetValue(null, false);
        }

        SetAlignmentValue();

        SystemParameters.StaticPropertyChanged += (sender, e) => {SetAlignmentValue();};
    }
}