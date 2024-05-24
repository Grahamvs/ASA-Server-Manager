using System.Collections.ObjectModel;
using System.ComponentModel;
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
    private readonly Func<Window, IToastService> _toastServiceFunc;
    private readonly IUpdateService _updateService;
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
    private IToastService _toastService;

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
        IUpdateService updateService,
        IProcessHelper processHelper,
        Func<Window, IToastService> toastServiceFunc,
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
        _updateService = updateService;
        _processHelper = processHelper;
        _toastServiceFunc = toastServiceFunc;
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
        ShowCustomMapsCommand = CreateBasedCommand(new ActionCommand(ExecuteShowCustomMapsCommand));
        ShowAboutWindowCommand = CreateBasedCommand(new ActionCommand(ExecuteShowAboutWindowCommand));

        SortListViewCommand = CreateBasedCommand(new ActionCommand<string>(ExecuteSortListViewCommand));

        BrowseClusterDirOverrideCommand = CreateBasedCommand(new ActionCommand(ExecuteBrowseClusterDirOverrideCommand));

        DonateCommand = new ActionCommand(ExecuteDonateCommand);
        CheckForAppUpdatesCommand = new ActionCommand(ExecuteCheckForAppUpdatesCommand);
        OpenFAQCommand = new ActionCommand(ExecuteOpenFAQCommand);
        OpenWikiCommand = new ActionCommand(ExecuteOpenWikiCommand);

        var serverBackupCommand = new ActionCommand(async () => await ExecuteServerBackupCommandAsync().ConfigureAwait(false), CanExecuteRunServerBackupCommand)
            .ObservesProperty(() => CanRunServerBackup);

        ServerBackupCommand = CreateBasedCommand(serverBackupCommand);

        var updateServerCommand = new ActionCommand(async () => await ExecuteUpdateServerCommandAsync().ConfigureAwait(false), CanExecuteUpdateServerCommand)
            .ObservesProperty(() => CanUpdateServer);

        UpdateServerCommand = CreateBasedCommand(updateServerCommand);

        var runCommand = new ActionCommand(async () => await ExecuteRunServerCommandAsync().ConfigureAwait(false), CanExecuteRunServerCommand)
            .ObservesProperty(() => CanRunServer)
            .ObservesProperty(() => SelectedMap)
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

    public IReadOnlyList<MapDetails> AvailableMaps => _mapService.AvailableMaps;

    public ICommand BrowseClusterDirOverrideCommand { get; }

    public bool CanRunServer => _serverHelper.CanRunServer;

    public bool CanRunServerBackup => _serverHelper.CanBackupServer;

    public bool CanUpdateServer => _serverHelper.CanUpdateServer;

    public ICommand CheckForAppUpdatesCommand { get; }

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

    public ICommand OpenFAQCommand { get; }

    public ICommand OpenWikiCommand { get; }

    public ICommand OpenFolderCommand => _serverHelper.OpenFolderCommand;

    public bool ProfileIsValid =>
        (CurrentProfile?.IsValid ?? false)
        && SelectedMap != null;

    public IEnumerable<string> RecentProfiles => _appSettingsService.RecentProfiles;

    public ICommand RunServerCommand { get; }

    public ICommand SaveAsCommand { get; }

    public ICommand SaveCommand { get; }

    public MapDetails SelectedMap
    {
        get => AvailableMaps.FirstOrDefault(map => map.ID == CurrentProfile.MapID);
        set => CurrentProfile.MapID = value?.ID;
    }

    public SelectableMod SelectedMod
    {
        get => _selectedMod;
        set => SetProperty(ref _selectedMod, value);
    }

    public ICommand ServerBackupCommand { get; }

    public ICommand ShowAboutWindowCommand { get; }

    public ICommand ShowAvailableModsCommand { get; }

    public ICommand ShowCustomMapsCommand { get; }

    public bool ShowModIDColumn => _appSettingsService.ShowModIDColumn;

    public bool ShowServerBackupCommand => _serverHelper.BackupExecutablePathIsValid;

    public ICommand ShowSettingsCommand { get; }

    public bool ShowUpdateCommand =>
        _appSettingsService.ServerType == ServerInstallType.SteamCMD
        && _serverHelper.SteamCmdPathIsValid;

    public ICommand SortListViewCommand { get; }

    public ICommand UpdateServerCommand { get; }

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
        _toastService = _toastServiceFunc(_viewService.GetWindow(this));

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
                .WherePropertyIs(nameof(IMapService.AvailableMaps))
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
            nameof(CurrentProfile),
            nameof(SelectedMap)
        );
    }

    protected override void OnUnload()
    {
        _serverHelper.StopServerMonitor();

        DisposeField(ref _subscriptions);
        DisposeField(ref _currentProfileSubscriptions);
        DisposeField(ref _modSubscriptions);
        DisposeField(ref _toastService);
    }

    #endregion

    #region Private Methods

    private bool CanExecuteRunServerBackupCommand() => CanRunServerBackup;

    private bool CanExecuteRunServerCommand() => ProfileIsValid && CanRunServer;

    private bool CanExecuteUpdateServerCommand() => CanUpdateServer;

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

    private void ExecuteCheckForAppUpdatesCommand() => _updateService.CheckForUpdates(true, true, _toastService);

    private void ExecuteDonateCommand()
    {
        var message = "Enjoying the software? It's on the house! If you want to contribute to my caffeine addiction by buying me a coffee, that'd be super cool. But if not, that's fine too!";

        var result = _dialogService.ShowMessage(message, "Donations welcomed", MessageBoxButton.YesNo);

        if (result != MessageBoxResult.Yes)
            return;

        _processHelper.OpenWeblink("https://www.paypal.me/slayerice09");
    }

    private void ExecuteLoadProfileCommand(CommandParameter parameter)
    {
        var workingDirectory = _applicationService.WorkingDirectory;

        // Check if we are loading a new profile, or loading one of the recent profiles.
        var fileName = parameter?.Parameter is string profile
            ? _fileSystemService.GetFullPath(profile, workingDirectory)
            : null;

        if (fileName.IsNullOrWhiteSpace() || !_fileSystemService.FileExists(fileName))
        {
            var ext = ServerProfileService.ProfileExtension;
            var description = ServerProfileService.ProfileDescription;

            var (folder, exists) = _serverHelper.GetFolder(ServerFolders.Profile);

            var initialDirectory = exists
                ? folder
                : workingDirectory;

            fileName = _dialogService.OpenFileDialog(
                $"{description}|*.{ext}",
                initialDirectory
            );
        }

        if (fileName.IsNullOrWhiteSpace())
            return;

        _serverProfileService.LoadProfile(fileName);

        RefreshModList();

        _toastService.ShowSuccess($"Profile '{_fileSystemService.GetFileNameWithoutExtension(_serverProfileService.CurrentFileName)}' loaded.");
    }

    private void ExecuteOpenFAQCommand() => _processHelper.OpenWeblink("https://github.com/Grahamvs/ASA-Server-Manager/blob/main/FAQ.md");

    private void ExecuteOpenWikiCommand() => _processHelper.OpenWeblink("https://ark.wiki.gg/wiki/Server_configuration");

    private async Task ExecuteRunServerCommandAsync()
    {
        if (!CanExecuteRunServerCommand())
            return;

        using var token = _isBusyHelper.GetToken();
        await _serverHelper.RunServerAsync(CurrentProfile).ConfigureAwait(false);
    }

    private async Task ExecuteServerBackupCommandAsync()
    {
        if (!CanExecuteRunServerBackupCommand())
            return;

        var task = _serverHelper.RunBackupExecutable();

        _toastService.ShowInformation("Running backup executable.");

        await task.ConfigureAwait(false);
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

        _toastService.ShowSuccess("Available mods updated.");
    }

    private void ExecuteShowCustomMapsCommand()
    {
        var result = _viewService.ShowViewDialog<ICustomMapsViewModel>(startupLocation: WindowStartupLocation.CenterOwner, owner: this);

        if (result != true)
            return;

        RaisePropertiesChanged(
            nameof(AvailableMaps),
            nameof(SelectedMap),
            nameof(ProfileIsValid)
        );

        _toastService.ShowSuccess("Custom maps updated.");
    }

    private void ExecuteShowSettingsCommand()
    {
        var result = _viewService.ShowViewDialog<ISettingsViewModel>(startupLocation: WindowStartupLocation.CenterOwner, owner: this);

        if (result != true)
            return;

        RaisePropertiesChanged(nameof(ShowUpdateCommand), nameof(ShowModIDColumn));

        _toastService.ShowSuccess("Settings saved.");
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

    private async Task ExecuteUpdateServerCommandAsync()
    {
        if (!CanExecuteUpdateServerCommand())
            return;

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
                .Subscribe(pattern => OnCurrentProfile_PropertyChanged(pattern.EventArgs)),
         };

        RaisePropertiesChanged(
            nameof(CurrentProfile),
            nameof(SelectedMap)
        );
    }

    private void OnCurrentProfile_PropertyChanged(PropertyChangedEventArgs patternEventArgs)
    {
        switch (patternEventArgs.PropertyName)
        {
            case nameof(IServerProfile.MapID):
                RaisePropertyChanged(nameof(SelectedMap));
                break;

            case nameof(IServerProfile.IsValid):
                RaisePropertyChanged(nameof(ProfileIsValid));
                break;

            case nameof(IServerProfile.HasChanges):
                RaisePropertyChanged(nameof(WindowTitle));
                break;
        }
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
            || CheckValue(mod.Comments);

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