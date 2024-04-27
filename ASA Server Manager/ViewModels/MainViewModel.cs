using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ASA_Server_Manager.Common;
using ASA_Server_Manager.Common.Commands;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Configs;
using ASA_Server_Manager.Interfaces.Helpers;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Interfaces.ViewModels;
using ASA_Server_Manager.Services;

namespace ASA_Server_Manager.ViewModels;

public class MainViewModel : WindowViewModel, IMainViewModel
{
    #region Private Fields

    private readonly IApplicationService _applicationService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly string _appTitle = "ASA Server Manager";
    private readonly IDialogService _dialogService;
    private readonly IFileSystemService _fileSystemService;
    private readonly TokenHelper _isBusyHelper;
    private readonly IMapService _mapService;
    private readonly IModService _modService;
    private readonly IProcessHelper _processHelper;
    private readonly IServerHelper _serverHelper;
    private readonly IServerProfileService _serverProfileService;
    private readonly IViewService _viewService;
    private IDisposable _commandToken;
    private CompositeDisposable _currentProfileSubscriptions;
    private ListSortDirection _lastDirection;
    private string _lastSortBy;
    private string _modFilterText;
    private ObservableCollection<SelectableMod> _modList;
    private ICollectionView _modListView;
    private CompositeDisposable _modSubscriptions;
    private SelectableMod _selectedMod;
    private CompositeDisposable _subscriptions;

    #endregion

    #region Public Constructors

    public MainViewModel(
        IApplicationService applicationService,
        IAppSettingsService appSettingsService,
        IServerProfileService serverProfileService,
        IMapService mapService,
        IServerHelper serverHelper,
        IModService modService,
        IFileSystemService fileSystemService,
        IDialogService dialogService,
        IProcessHelper processHelper,
        IViewService viewService
    )
    {
        _applicationService = applicationService;
        _appSettingsService = appSettingsService;
        _serverProfileService = serverProfileService;
        _mapService = mapService;
        _serverHelper = serverHelper;
        _modService = modService;
        _fileSystemService = fileSystemService;
        _dialogService = dialogService;
        _processHelper = processHelper;
        _viewService = viewService;

        _isBusyHelper = new TokenHelper(_ => RaisePropertyChanged(nameof(IsBusy)));

        var baseCommand = new ActionCommand(() => { }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

        var autosaveCommand = new ActionCommand<CommandParameter>(cp => ((Action)(ForceSaveProfile() == null ? cp.Cancel : null))?.Invoke());

        SaveCommand = CreateBasedCommand(new ActionCommand(() => SaveProfile(_serverProfileService.CurrentFilePath)));
        SaveAsCommand = CreateBasedCommand(new ActionCommand(() => SaveProfile(null)));

        LoadProfileCommand = CreateBasedCommand(autosaveCommand, new ActionCommand<CommandParameter>(ExecuteLoadProfileCommand));

        NewProfileCommand = CreateBasedCommand(new ActionCommand(() => _serverProfileService.ResetProfile()));

        ShowSettingsCommand = CreateBasedCommand(new ActionCommand(ExecuteShowSettingsCommand));
        ShowAvailableModsCommand = CreateBasedCommand(new ActionCommand(ExecuteShowAvailableModsCommand));
        ShowAboutWindowCommand = CreateBasedCommand(new ActionCommand(ExecuteShowAboutWindowCommand));

        SortListViewCommand = CreateBasedCommand(new ActionCommand<string>(ExecuteSortListViewCommand));

        BrowseClusterDirOverrideCommand = CreateBasedCommand(new ActionCommand(ExecuteBrowseClusterDirOverrideCommand));

        DonateCommand = new ActionCommand(ExecuteDonateCommand);

        var serverBackupCommand = new ActionCommand(async () => await ExecuteServerBackupCommandAsync().ConfigureAwait(false), CanExecuteRunServerBackupCommand)
            .ObservesProperty(() => CanRunServerBackup);

        ServerBackupCommand = CreateBasedCommand(serverBackupCommand);

        var updateCommand = new ActionCommand(async () => await ExecuteUpdateCommandAsync().ConfigureAwait(false), CanExecuteUpdateCommand)
            .ObservesProperty(() => CanUpdateServer);

        UpdateCommand = CreateBasedCommand(updateCommand);

        var runCommand = new ActionCommand(async () => await ExecuteRunServerCommandAsync().ConfigureAwait(false), CanExecuteRunServerCommand)
            .ObservesProperty(() => CanRunServer)
            .ObservesProperty(() => ProfileIsValid);

        RunServerCommand = CreateBasedCommand(autosaveCommand, runCommand);

        return;

        //// Local Functions \\\\

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

    #endregion

    #region Public Properties

    public IReadOnlyList<string> AvailableMaps => _mapService.AvailableIDs;

    public ICommand BrowseClusterDirOverrideCommand { get; }

    public bool CanRunServer => _serverHelper.CanRunServer;

    public bool CanRunServerBackup => _serverHelper.CanBackupServer;

    public bool CanUpdateServer => _serverHelper.CanUpdateServer;

    public IServerProfile CurrentProfile => _serverProfileService.CurrentProfile;

    public ICommand DonateCommand { get; }

    public bool HasRecentProfiles => RecentProfiles.Any();

    public bool IsBusy => _isBusyHelper.HasTokens;

    public ICommand LoadProfileCommand { get; }

    public string ModFilterText
    {
        get => _modFilterText;
        set => SetProperty(ref _modFilterText, value, ModListView.Refresh);
    }

    public ICollectionView ModListView => _modListView;

    public ICommand NewProfileCommand { get; }

    public ICommand OpenFolderCommand => _serverHelper.OpenFolderCommand;

    public bool ProfileIsValid => CurrentProfile?.IsValid ?? false;

    public IEnumerable<string> RecentProfiles => _appSettingsService.RecentProfiles;

    public ICommand RunServerCommand { get; }

    public ICommand SaveAsCommand { get; }

    public ICommand SaveCommand { get; }

    public SelectableMod SelectedMod
    {
        get => _selectedMod;
        set => SetProperty(ref _selectedMod, value);
    }

    public ICommand ServerBackupCommand { get; }

    public ICommand ShowAboutWindowCommand { get; }

    public ICommand ShowAvailableModsCommand { get; }

    public bool ShowServerBackupCommand => _serverHelper.BackupExecutablePathIsValid;

    public ICommand ShowSettingsCommand { get; }

    public bool ShowUpdateCommand =>
        _appSettingsService.ServerType == ServerInstallType.SteamCMD
        && _serverHelper.SteamCmdPathIsValid;

    public ICommand SortListViewCommand { get; }

    public ICommand UpdateCommand { get; }

    public string WindowTitle =>
        _serverProfileService.CurrentFileName.IsNullOrWhiteSpace()
            ? _appTitle
            : $"{_appTitle}: {_serverProfileService.CurrentFileName}{(CurrentProfile.HasChanges ? "*" : string.Empty)}";

    #endregion

    #region Public Methods

    public override bool OnWindowClosing() => ForceSaveProfile() is not null;

    #endregion

    #region Protected Methods

    protected override void OnLoad()
    {
        _subscriptions =
        [
            _serverProfileService
                .FromPropertyChangedPattern()
                .WherePropertyIs(nameof(IServerProfileService.CurrentProfile))
                .Subscribe(_ => OnCurrentProfile_Changed()),

            _serverProfileService
                .FromPropertyChangedPattern()
                .WherePropertyIs(nameof(IServerProfileService.CurrentFileName))
                .Subscribe(_ => RaisePropertyChanged(nameof(WindowTitle))),

            _mapService
                .FromPropertyChangedPattern()
                .WherePropertyIs(nameof(IMapService.AvailableIDs))
                .Subscribe(_ => RaisePropertyChanged(nameof(AvailableMaps))),

            _appSettingsService
                .FromPropertyChangedPattern()
                .WherePropertyIs(nameof(IAppSettingsService.RecentProfiles))
                .Subscribe(_ => RaisePropertiesChanged(nameof(RecentProfiles), nameof(HasRecentProfiles))),

            _serverHelper
                .FromPropertyChangedPattern()
                .Subscribe(pattern => OnArkServerHelper_PropertyChanged(pattern.EventArgs)),
        ];

        RefreshModList();

        _serverHelper.StartServerMonitor();

        OnCurrentProfile_Changed();

        RaisePropertiesChanged(
            nameof(ShowUpdateCommand),
            nameof(WindowTitle),
            nameof(AvailableMaps),
            nameof(CurrentProfile)
        );
    }

    protected override void OnUnload()
    {
        DisposeField(ref _subscriptions);
        DisposeField(ref _currentProfileSubscriptions);
        DisposeField(ref _modSubscriptions);

        _serverHelper.StopServerMonitor();

        ForceSaveProfile();
    }

    #endregion

    #region Private Methods

    private bool CanExecuteRunServerBackupCommand() => CanRunServerBackup;

    private bool CanExecuteRunServerCommand() => ProfileIsValid && CanRunServer;

    private bool CanExecuteUpdateCommand() => CanUpdateServer;

    private void ExecuteBrowseClusterDirOverrideCommand()
    {
        var initialDirectory = CurrentProfile.ClusterDirOverride
            ?? _fileSystemService.GetDirectoryName(_appSettingsService.ServerPath);

        var folder = _dialogService.OpenFolderDialog(initialDirectory);

        if (!folder.IsNullOrWhiteSpace())
        {
            CurrentProfile.ClusterDirOverride = folder;
        }
    }

    private void ExecuteDonateCommand()
    {
        var message = "Enjoying the software? It's on the house! If you want to contribute to my caffeine addiction by buying me a coffee, that'd be super cool. But if not, that's fine too!";

        var result = _dialogService.ShowMessage(message, "Donations welcomed", MessageBoxButton.YesNo);

        if (result != MessageBoxResult.Yes)
            return;

        var info = new ProcessStartInfo("https://www.paypal.me/slayerice09") { UseShellExecute = true };
        _processHelper.CreateProcess(info).Start();
    }

    private void ExecuteLoadProfileCommand(CommandParameter parameter)
    {
        var fileName = parameter.Parameter is string val
            ? _fileSystemService.GetFullPath(val, _applicationService.WorkingDirectory)
            : null;

        if (fileName.IsNullOrWhiteSpace() || !_fileSystemService.FileExists(fileName))
        {
            var file = _serverProfileService.CurrentFilePath;

            var ext = ServerProfileService.ProfileExtension;
            var description = ServerProfileService.ProfileDescription;

            var initialDirectory = !file.IsNullOrWhiteSpace()
                ? _fileSystemService.GetDirectoryName(file)
                : _applicationService.WorkingDirectory;

            fileName = _dialogService.OpenFileDialog(
                $"{description}|*.{ext}",
                initialDirectory
            );
        }

        if (!fileName.IsNullOrWhiteSpace())
        {
            _serverProfileService.LoadProfile(fileName);

            RefreshModList();
        }
    }

    private async Task ExecuteRunServerCommandAsync()
    {
        if (!CanExecuteRunServerCommand())
            return;

        await _serverHelper.RunServerAsync(CurrentProfile).ConfigureAwait(false);
    }

    private async Task ExecuteServerBackupCommandAsync()
    {
        if (!CanExecuteRunServerBackupCommand())
            return;

        await _serverHelper.RunBackupExecutable().ConfigureAwait(false);
    }

    private void ExecuteShowAboutWindowCommand()
    {
        _viewService.ShowViewDialog<IAboutViewModel>(startupLocation: WindowStartupLocation.CenterOwner, owner: this);
    }

    private void ExecuteShowAvailableModsCommand()
    {
        var result = _viewService.ShowViewDialog<IAvailableModsViewModel>(startupLocation: WindowStartupLocation.CenterOwner, owner: this);

        if (result != true)
            return;

        RefreshModList();
    }

    private void ExecuteShowSettingsCommand()
    {
        _viewService.ShowViewDialog<ISettingsViewModel>(startupLocation: WindowStartupLocation.CenterOwner, owner: this);

        RaisePropertyChanged(nameof(ShowUpdateCommand));
    }

    private void ExecuteSortListViewCommand(string sortBy)
    {
        var direction = ListSortDirection.Ascending;

        // If already sorted by this column, reverse the sort direction
        if (_lastSortBy == sortBy)
        {
            direction = _lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        _modListView.SortDescriptions.Clear();
        _modListView.SortDescriptions.Add(new SortDescription(sortBy, direction));

        _lastSortBy = sortBy;
        _lastDirection = direction;
    }

    private async Task ExecuteUpdateCommandAsync()
    {
        if (!CanExecuteUpdateCommand())
            return;

        //using var token = _isBusyHelper.GetToken();

        await _serverHelper.UpdateServerAsync().ConfigureAwait(false);
    }

    private bool? ForceSaveProfile()
    {
        var filePath = _serverProfileService.CurrentFilePath;
        var hasChanges = _serverProfileService.CurrentProfile.HasChanges;

        if (!_serverProfileService.IsFileLoaded || !hasChanges || filePath.IsNullOrWhiteSpace())
            return false;

        if (_appSettingsService.AutoSaveProfile)
        {
            SaveProfile(filePath);
            return true;
        }

        var message = "You have unsaved changes.\r\nDo you wish to save them before proceeding?";

        switch (_dialogService.ShowMessage(message, buttons: MessageBoxButton.YesNoCancel))
        {
            case MessageBoxResult.Yes:
                SaveProfile(filePath);
                return true;

            case MessageBoxResult.No:
                return false;

            default:
                return null;
        }
    }

    private void OnArkServerHelper_PropertyChanged(PropertyChangedEventArgs eventArgs)
    {
        switch (eventArgs.PropertyName)
        {
            case nameof(IServerHelper.SteamCmdPathIsValid):
                RaisePropertiesChanged(nameof(ShowUpdateCommand));
                break;

            case nameof(IServerHelper.CanUpdateServer):
                RaisePropertyChanged(nameof(CanUpdateServer));
                break;

            case nameof(IServerHelper.UpdatingServer):
                RaisePropertyChanged(nameof(CanUpdateServer));
                break;

            case nameof(IServerHelper.CanRunServer):
                RaisePropertyChanged(nameof(CanRunServer));
                break;

            case nameof(IServerHelper.CanBackupServer):
                RaisePropertyChanged(nameof(CanRunServerBackup));
                break;

            case nameof(IServerHelper.BackupExecutablePathIsValid):
                RaisePropertyChanged(nameof(ShowServerBackupCommand));
                break;
        }
    }

    private void OnCurrentProfile_Changed()
    {
        DisposeField(ref _currentProfileSubscriptions);

        var currentProfile = CurrentProfile;

        _currentProfileSubscriptions = new()
        {
            currentProfile
                .FromPropertyChangedPattern()
                .WherePropertyIs(nameof(IServerProfile.IsValid))
                .Subscribe(_ => RaisePropertyChanged(nameof(ProfileIsValid))),

            currentProfile
                .FromPropertyChangedPattern()
                .WherePropertyIs(nameof(IServerProfile.HasChanges))
                .Subscribe(_ => RaisePropertiesChanged(nameof(WindowTitle))),
        };

        RaisePropertyChanged(nameof(CurrentProfile));
    }

    private bool OnModsFilter(object obj)
    {
        if (obj is not SelectableMod mod)
            return false;

        if (ModFilterText.IsNullOrEmpty())
            return true;

        var mode = mod.IsPassive
            ? ModMode.Passive
            : mod.IsSelected
                ? ModMode.Enabled
                : ModMode.Disabled;

        return CheckValue(mode.ToString())
            || CheckValue(mod.ID.ToString())
            || CheckValue(mod.Name)
            || CheckValue(mod.Description);

        //// Local Functions \\\\

        bool CheckValue(string value) => (value ?? string.Empty).Contains(ModFilterText, StringComparison.OrdinalIgnoreCase);
    }

    private void RefreshModList()
    {
        DisposeField(ref _modSubscriptions);

        _modSubscriptions = [];

        _modList = new ObservableCollection<SelectableMod>(
            _modService
                .AvailableModsList.Select(
                    mod =>
                    {
                        var currentMode = CurrentProfile.SelectedMods.GetValueOrDefault(mod.ID);

                        var selectableMod = new SelectableMod(mod)
                        {
                            IsSelected = currentMode != ModMode.Disabled,
                            IsPassive = currentMode == ModMode.Passive,
                        };

                        _modSubscriptions.Add(
                            selectableMod
                                .FromPropertyChangedPattern()
                                .WherePropertiesAre(nameof(SelectableMod.IsSelected), nameof(SelectableMod.IsPassive))
                                .Sample(TimeSpan.FromMilliseconds(250))
                                .Subscribe(
                                    _ =>
                                    {
                                        var mode = selectableMod.IsSelected
                                            ? selectableMod.IsPassive ? ModMode.Passive : ModMode.Enabled
                                            : ModMode.Disabled;

                                        CurrentProfile.SetModMode(selectableMod.ID, mode);
                                    }
                                )
                        );

                        return selectableMod;
                    }
                )
        );

        var viewSource = CollectionViewSource.GetDefaultView(_modList);
        viewSource.Filter = OnModsFilter;
        _modListView = viewSource;

        viewSource.Refresh();

        RaisePropertyChanged(nameof(ModListView));
    }

    private void SaveProfile(string filePath)
    {
        using var token = _isBusyHelper.GetToken();

        try
        {
            var ext = ServerProfileService.ProfileExtension;
            var description = ServerProfileService.ProfileDescription;
            var file = _serverProfileService.CurrentFilePath;

            filePath ??= _dialogService.SaveFileDialog(ext, $"{description}|*.{ext}", file);

            if (!filePath.IsNullOrWhiteSpace())
            {
                _serverProfileService.SaveProfile(filePath);
            }
        }
        catch (Exception e)
        {
            _dialogService.ShowErrorMessage($"An error occurred while saving:\r\n\r\n{e.Message}");
        }
    }

    #endregion
}