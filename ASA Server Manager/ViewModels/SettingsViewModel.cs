using System.Windows;
using System.Windows.Input;
using ASA_Server_Manager.Common;
using ASA_Server_Manager.Common.Commands;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Helpers;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Interfaces.ViewModels;
using Range = ASA_Server_Manager.Common.Range;

namespace ASA_Server_Manager.ViewModels;

public class SettingsViewModel : WindowViewModel, ISettingsViewModel
{
    #region Private Fields

    private readonly IApplicationService _applicationService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly IDialogService _dialogService;
    private readonly IFileSystemService _fileSystemService;
    private readonly TokenHelper _isBusyHelper;
    private readonly IProgress<double> _progress;
    private readonly IServerHelper _serverHelper;
    private bool _autoSaveProfile;
    private string _backupExecutablePath;
    private string _busyMessage;
    private double _progressValue;
    private IDisposable _commandToken;
    private bool _progressIsIndeterminate;
    private int _recentProfilesLimit;
    private ServerInstallType _selectedServerType;
    private string _serverPath;
    private bool _showModIDColumn;
    private string _steamCmdPath;
    private bool _updateOnFirstRun;

    #endregion

    #region Public Constructors

    public SettingsViewModel(
        IApplicationService applicationService,
        IAppSettingsService appSettingsService,
        IFileSystemService fileSystemService,
        IServerHelper serverHelper,
        IDialogService dialogService
    )
    {
        _applicationService = applicationService;
        _appSettingsService = appSettingsService;
        _fileSystemService = fileSystemService;
        _serverHelper = serverHelper;
        _dialogService = dialogService;

        _isBusyHelper = new TokenHelper(_ => RaisePropertyChanged(nameof(IsBusy)));
        _progress = new Progress<double>(value => ProgressValue = value);

        var baseCommand = new ActionCommand(() => { }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

        var serverTypeBaseCommand = new ActionCommand(() => { })
            .ObservesProperty(() => SelectedServerType)
            .ObservesProperty(() => SteamCmdPath)
            .ObservesProperty(() => ServerPath);
        ;

        BrowseFileCommand = CreateBasedCommand(new ActionCommand<FilePathEnum>(ExecuteBrowseFileCommand));

        SaveCommand = CreateBasedCommand(
            serverTypeBaseCommand,
            new ActionCommand(ExecuteSaveCommand, CanExecuteSaveCommand)
        );

        InstallSteamCmdCommand = CreateBasedCommand(
            serverTypeBaseCommand,
            new ActionCommand(async () => await ExecuteInstallSteamCmdCommandAsync(), CanExecuteInstallSteamCmdCommandAsync)
        );

        return;

        CompositeCommand CreateBasedCommand(params ICommand[] commands)
        {
            var commandList = new List<ICommand> { baseCommand };
            commandList.AddRange(commands);

            return new(commandList.ToArray())
            {
                ErrorHandler = exception => _dialogService.ShowErrorMessage(exception.Message),
                BeginExecuteAction = param =>
                {
                    (param as CommandParameter)?.Reset();
                    _commandToken = _isBusyHelper.GetToken();
                },
                EndExecuteAction = _ => _commandToken?.Dispose(),
            };
        }
    }

    private bool CanExecuteInstallSteamCmdCommandAsync() => SelectedServerType == ServerInstallType.SteamCMD;

    private bool CanExecuteSaveCommand() =>
        SelectedServerType switch
        {
            ServerInstallType.SteamCMD => !SteamCmdPath.IsNullOrWhiteSpace(),
            ServerInstallType.Standalone => !ServerPath.IsNullOrWhiteSpace(),
            _ => false
        };

    #endregion

    #region Public Properties

    public bool AutoSaveProfile
    {
        get => _autoSaveProfile;
        set => SetProperty(ref _autoSaveProfile, value);
    }

    public string BackupExecutablePath
    {
        get => _backupExecutablePath;
        set => SetProperty(ref _backupExecutablePath, value);
    }

    public ICommand BrowseFileCommand { get; set; }

    public string BusyMessage
    {
        get => _busyMessage;
        private set => SetProperty(ref _busyMessage, value);
    }

    public double ProgressValue
    {
        get => _progressValue;
        set => SetProperty(ref _progressValue, value);
    }

    public ICommand InstallSteamCmdCommand { get; }

    public bool IsBusy => _isBusyHelper.HasTokens;

    public bool ProgressIsIndeterminate
    {
        get => _progressIsIndeterminate;
        set => SetProperty(ref _progressIsIndeterminate, value);
    }

    public int RecentProfilesLimit
    {
        get => _recentProfilesLimit;
        set => SetProperty(ref _recentProfilesLimit, Range.SetInRange(value, 0, 100));
    }

    public ICommand SaveCommand { get; }

    public ServerInstallType SelectedServerType
    {
        get => _selectedServerType;
        set => SetProperty(ref _selectedServerType, value);
    }

    public string ServerPath
    {
        get => _serverPath;
        set => SetProperty(ref _serverPath, value);
    }

    public bool ShowModIDColumn
    {
        get => _showModIDColumn;
        set => SetProperty(ref _showModIDColumn, value);
    }

    public string SteamCmdPath
    {
        get => _steamCmdPath;
        set => SetProperty(ref _steamCmdPath, value);
    }

    public bool UpdateOnFirstRun
    {
        get => _updateOnFirstRun;
        set => SetProperty(ref _updateOnFirstRun, value);
    }

    public string WindowTitle => $"{_applicationService.ExeName}: Settings";

    #endregion

    #region Public Methods

    public override bool OnWindowClosing() => !IsBusy;

    #endregion

    #region Protected Methods

    protected override void OnLoad()
    {
        AutoSaveProfile = _appSettingsService.AutoSaveProfile;
        BackupExecutablePath = _appSettingsService.BackupExecutablePath;
        RecentProfilesLimit = _appSettingsService.RecentProfilesLimit;
        SelectedServerType = _appSettingsService.ServerType;
        ServerPath = _appSettingsService.ServerPath;
        ShowModIDColumn = _appSettingsService.ShowModIDColumn;
        SteamCmdPath = _appSettingsService.SteamCmdPath;
        UpdateOnFirstRun = _appSettingsService.UpdateOnFirstRun;
    }

    protected override void OnUnload()
    {
    }

    #endregion

    #region Private Methods

    private void ExecuteBrowseFileCommand(FilePathEnum filePathEnum)
    {
        string filter;
        string fileName;
        string title;
        Action<string> setPath;

        switch (filePathEnum)
        {
            case FilePathEnum.SteamCmd:
                filter = "SteamCMD|SteamCMD.exe";
                fileName = "SteamCMD.exe";
                title = "Select SteamCMD.exe";
                setPath = path => SteamCmdPath = path;
                break;

            case FilePathEnum.ASAServer:
                filter = "ArkAscendedServer|*.exe";
                fileName = "ArkAscendedServer.exe";
                title = "Select ArkAscendedServer.exe";
                setPath = path => ServerPath = path;
                break;

            case FilePathEnum.BackupScript:
                filter = "Executables|*.bat;*.cmd;*.exe|All Files|*.*";
                fileName = null;
                title = "Select Server Backup executable:";
                setPath = path => BackupExecutablePath = path;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(filePathEnum), filePathEnum, null);
        }

        var result = _dialogService.OpenFileDialog(filter, fileName: fileName, title: title);

        if (result != null)
        {
            setPath(result);
        }
    }

    private async Task ExecuteInstallSteamCmdCommandAsync()
    {
        using var busyToken = _isBusyHelper.GetToken();
        _progress.Report(-1);

        const string Message = "This will download and install Ark Survival Ascended's dedicated server using SteamCMD, and requires at least 11GB of data!.\r\n\r\nDo you wish to continue?";
        const string Title = "Install Ark Survival Ascended server";

        var result = _dialogService.ShowMessage(Message, Title, MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        string arkServerPath = null;

        try
        {
            var dir = _dialogService.OpenFolderDialog(_applicationService.WorkingDirectory, "Select SteamCMD install folder");

            if (dir.IsNullOrWhiteSpace())
            {
                _dialogService.ShowMessage("Installation canceled: No folder selected.", Title);
                return;
            }

            ResetProgress("Downloading SteamCMD...", false);

            var exePath = await _serverHelper.DownloadSteamCmdAsync(dir, _progress);

            ResetProgress("Installing Server...", true);

            SteamCmdPath = _fileSystemService.GetRelativePath(_applicationService.WorkingDirectory, exePath);

            // Set the SteamCMD path in the settings, so we can run the update command.
            _appSettingsService.SteamCmdPath = SteamCmdPath;

            await _serverHelper.UpdateServerAsync();

            ServerPath = Defaults.ASARelativePath;

            arkServerPath = _fileSystemService.GetFullPath(ServerPath, _fileSystemService.GetDirectoryName(exePath));
        }
        catch (Exception e)
        {
            _dialogService.ShowErrorMessage(e.Message);
        }

        ResetProgress(null, false);

        if (arkServerPath.IsNullOrWhiteSpace() || !_fileSystemService.FileExists(arkServerPath))
        {
            _dialogService.ShowErrorMessage("Ark Survival Ascended server executable not found!\r\n\r\nIf you did not cancel the installation, you may need to manually install it!", Title);
        }
        else
        {
            _dialogService.ShowMessage("SteamCMD has been installed.\r\n\r\nNote: You may need to run the server first before the save folder and .ini files are generated!", Title, icon: MessageBoxImage.Information);
        }

        return;

        //// Local Functions \\\\

        void ResetProgress(string message, bool indeterminate)
        {
            BusyMessage = message;
            ProgressIsIndeterminate = indeterminate;
            _progress.Report(indeterminate ? 1 : 0);
        }
    }

    private void ExecuteSaveCommand()
    {
        _appSettingsService.AutoSaveProfile = AutoSaveProfile;
        _appSettingsService.BackupExecutablePath = BackupExecutablePath;
        _appSettingsService.RecentProfilesLimit = RecentProfilesLimit;
        _appSettingsService.ServerType = SelectedServerType;
        _appSettingsService.ShowModIDColumn = ShowModIDColumn;
        _appSettingsService.SteamCmdPath = SteamCmdPath;
        _appSettingsService.UpdateOnFirstRun = UpdateOnFirstRun;

        _appSettingsService.ServerPath =
            SelectedServerType == ServerInstallType.SteamCMD
                ? Defaults.ASARelativePath
                : ServerPath;

        _appSettingsService.SaveSettings();

        // This feels a bit hacky, but for now we'll clear the busy tokens, so we can close the window.
        _isBusyHelper.Clear();

        RaiseCloseRequested(true);
    }

    #endregion
}