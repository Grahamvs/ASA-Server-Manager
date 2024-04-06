using System.Windows;
using System.Windows.Controls;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Encryption;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.ViewModels;

namespace ASA_Server_Manager.Views;

public partial class MainWindow
{
    #region Private Fields

    private IDisposable _configChangedSub;
    private IDisposable _passwordSubscriptions;

    #endregion

    #region Public Constructors

    public MainWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region Private Methods

    private void OnAdminPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.CurrentProfile.AdminPassword = StringEncryptor.Encrypt(AdminPasswordBox.Password);
    }

    private void OnProfileChanged()
    {
        DisposeField(ref _passwordSubscriptions);

        var viewModel = ViewModel;

        var config = viewModel?.CurrentProfile;

        if (config != null)
        {
            _passwordSubscriptions = viewModel
                .CurrentProfile
                .FromPropertyChangedPattern()
                .WherePropertiesAre(nameof(ServerProfile.ServerPassword), nameof(ServerProfile.AdminPassword))
                .Subscribe(
                    pattern =>
                    {
                        switch (pattern.EventArgs.PropertyName)
                        {
                            case nameof(ServerProfile.AdminPassword):
                                SetPassword(AdminPasswordBox, config.AdminPassword);
                                break;

                            case nameof(ServerProfile.ServerPassword):
                                SetPassword(ServerPasswordBox, config?.ServerPassword);
                                break;
                        }
                    }
                );
        }

        SetPassword(AdminPasswordBox, config?.AdminPassword);
        SetPassword(ServerPasswordBox, config?.ServerPassword);
    }

    private void SetPassword(PasswordBox box, string password)
    {
        var decrypted = StringEncryptor.Decrypt(password);

        if (box.Password != decrypted)
        {
            box.Password = decrypted;
        }
    }

    private void OnServerPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.CurrentProfile.ServerPassword = StringEncryptor.Encrypt(ServerPasswordBox.Password);
    }

    protected override void OnViewModelChanged()
    {
        DisposeField(ref _configChangedSub);

        if (ViewModel != null)
        {
            _configChangedSub = ViewModel
                .FromPropertyChangedPattern()
                .WherePropertyIs(nameof(IMainViewModel.CurrentProfile))
                .Subscribe(_ => OnProfileChanged());
        }

        OnProfileChanged();
    }

    #endregion
}