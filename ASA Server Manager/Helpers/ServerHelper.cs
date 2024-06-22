using System.IO;
using System.IO.Compression;
using System.Reactive.Linq;
using System.Windows;
using ASA_Server_Manager.Common;
using ASA_Server_Manager.Common.Commands;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Common.Commands;
using ASA_Server_Manager.Interfaces.Configs;
using ASA_Server_Manager.Interfaces.Helpers;
using ASA_Server_Manager.Interfaces.Services;

namespace ASA_Server_Manager.Helpers;

public class ServerHelper : BindableBase, IServerHelper
{
    #region Private Fields

    private const string ASASteamServerID = "2430930";
    private readonly IAppSettingsService _appSettingsService;
    private readonly IDialogService _dialogService;
    private readonly IDownloadHelper _downloadHelper;
    private readonly IFileSystemService _fileSystemService;
    private readonly IModService _modService;
    private readonly IProcessHelper _processHelper;
    private readonly IServerProfileService _serverProfileService;
    private readonly string _workingDirectory;
    private ProcessMonitor _asaServerMonitor;
    private bool _backingUpServer;
    private IDisposable _serverMonitorSubscription;
    private ProcessMonitor _steamCmdMonitor;
    private IDisposable _steamCmdMonitorSubscription;
    private int _updateCount;
    private bool _updatingServer;

    #endregion

    #region Public Constructors

    public ServerHelper(
        IApplicationService applicationService,
        IAppSettingsService appSettingsService,
        IFileSystemService fileSystemService,
        IDialogService dialogService,
        IServerProfileService serverProfileService,
        IModService modService,
        IProcessHelper processHelper,
        IDownloadHelper downloadHelper
    )
    {
        _appSettingsService = appSettingsService;
        _fileSystemService = fileSystemService;
        _dialogService = dialogService;
        _serverProfileService = serverProfileService;
        _modService = modService;
        _processHelper = processHelper;
        _downloadHelper = downloadHelper;

        var openFolderCommand = new ActionCommand<ServerFolders>(ExecuteOpenFolderCommand, folder => GetFolder(folder).Exists);
        OpenFolderCommand = openFolderCommand;

        var openArkConfigFileCommand = new ActionCommand<ArkConfigFiles>(ExecuteOpenArkConfigFileCommand);
        OpenArkConfigFileCommand = openArkConfigFileCommand;

        _appSettingsService
            .FromPropertyChangedPattern()
            .WherePropertyIs(nameof(IAppSettingsService.SteamCmdPath))
            .Subscribe(_ => OnSteamCmdPathChanged());

        _appSettingsService
            .FromPropertyChangedPattern()
            .WherePropertyIs(nameof(IAppSettingsService.ServerPath))
            .Subscribe(_ => OnServerPathChanged());

        _appSettingsService
            .FromPropertyChangedPattern()
            .WherePropertyIs(nameof(IAppSettingsService.BackupExecutablePath))
            .Subscribe(_ => RaisePropertiesChanged(nameof(BackupExecutablePathIsValid), nameof(CanBackupServer)));

        _appSettingsService
            .FromPropertyChangedPattern()
            .WherePropertiesAre(
                nameof(IAppSettings.SteamCmdPath),
                nameof(IAppSettings.ServerPath),
                nameof(IAppSettings.BackupExecutablePath)
            )
            .Merge(
                _serverProfileService
                    .FromPropertyChangedPattern()
                    .WherePropertiesAre(nameof(IServerProfileService.CurrentFilePath))
            )
            .Sample(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => openFolderCommand.RaiseCanExecuteChanged());

        _appSettingsService
            .FromPropertyChangedPattern()
            .WherePropertiesAre(
                nameof(IAppSettings.SteamCmdPath),
                nameof(IAppSettings.ServerPath)
            )
            .Sample(TimeSpan.FromMilliseconds(300))
            .Subscribe(_ => openArkConfigFileCommand.RaiseCanExecuteChanged());

        _workingDirectory = applicationService.WorkingDirectory;

        OnSteamCmdPathChanged();
        OnServerPathChanged();
    }

    #endregion

    #region Public Properties

    public bool BackingUpServer
    {
        get => _backingUpServer;
        private set => SetProperty(ref _backingUpServer, value, UpdateCanStatuses);
    }

    public bool BackupExecutablePathIsValid => IsFileValid(BackupExecutablePath);

    public bool CanBackupServer =>
        BackupExecutablePathIsValid
        && !UpdatingServer
        && !BackingUpServer
        && !ServerRunning;

    public bool CanRunServer =>
        ServerPathIsValid
        && !UpdatingServer
        && !BackingUpServer;

    public bool CanUpdateServer =>
        SteamCmdPathIsValid
        && !_steamCmdMonitor.IsRunning
        && !UpdatingServer
        && !BackingUpServer
        && !ServerRunning;

    public ICommand<ServerFolders> OpenFolderCommand { get; }

    public ICommand<ArkConfigFiles> OpenArkConfigFileCommand { get; }

    public bool SteamCmdPathIsValid => IsFileValid(SteamCmdPath);

    public bool UpdatingServer
    {
        get => _updatingServer;
        private set => SetProperty(ref _updatingServer, value, UpdateCanStatuses);
    }

    #endregion

    #region Private Properties

    private string BackupExecutablePath =>
        _appSettingsService.BackupExecutablePath is { } path
            ? _fileSystemService.GetFullPath(path, SteamCmdFolder ?? _workingDirectory)
            : null;

    private string ServerPath =>
        _appSettingsService.ServerPath is { } path
            ? _fileSystemService.GetFullPath(path, SteamCmdFolder ?? _workingDirectory)
            : null;

    private bool ServerPathIsValid => IsFileValid(ServerPath);

    private bool ServerRunning => _asaServerMonitor?.IsRunning ?? false;

    private string SteamCmdFolder =>
        SteamCmdPath is { } path
            ? _fileSystemService.GetDirectoryName(path)
            : null;

    private string SteamCmdPath =>
        _appSettingsService.SteamCmdPath is { } path
            ? _fileSystemService.GetFullPath(path, _workingDirectory)
            : null;

    #endregion

    #region Public Methods

    public async Task<string> DownloadSteamCmdAsync(string steamCmdFolder, IProgress<double> progress = null, CancellationToken? cancellationToken = null)
    {
        // Remove any previous copies of steamcmd.exe
        var exe = "steamcmd.exe";
        var steamCmdPath = _fileSystemService.Combine(steamCmdFolder, exe);

        if (_fileSystemService.FileExists(steamCmdPath))
        {
            _fileSystemService.DeleteFile(steamCmdPath);
        }

        // Download steamcmd.zip
        const string url = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";

        using var stream = new MemoryStream();
        await _downloadHelper.DownloadFileToStream(url, stream, progress, cancellationToken);

        stream.Position = 0;

        using var zipArchive = new ZipArchive(stream);
        zipArchive.ExtractToDirectory(steamCmdFolder);

        return steamCmdPath;
    }

    public (string Folder, bool Exists) GetFolder(ServerFolders folder)
    {
        var dir = folder switch
        {
            ServerFolders.SteamCmd => SteamCmdFolder,
            ServerFolders.ServerPath => ServerFolder(),
            ServerFolders.ServerSave => _fileSystemService.GetFullPath("..\\..\\Saved", ServerFolder()),
            ServerFolders.Profile => GetParentFolder(_serverProfileService.CurrentFilePath, _workingDirectory),
            ServerFolders.BackupExe => GetParentFolder(BackupExecutablePath, _workingDirectory),
            _ => null
        };

        return (dir, dir is { } && _fileSystemService.DirectoryExists(dir));
    }

    public async Task RunBackupExecutable()
    {
        BackingUpServer = true;

        await ExecuteAndWaitForProcessAsync(_appSettingsService.BackupExecutablePath).ConfigureAwait(false);

        BackingUpServer = false;
    }

    public async Task RunServerAsync(IServerProfile profile)
    {
        if (_appSettingsService.UpdateOnFirstRun
            && CanUpdateServer
            && _updateCount++ == 0)
        {
            await UpdateServerAsync().ConfigureAwait(false);
        }

        if (!CanRunServer)
            return;

        var argument = new ServerArgumentBuilder().Build(profile, _modService.AvailableModsList);

        var process = _processHelper.CreateProcess(ServerPath, argument);
        process.Start();

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
    }

    public void StartServerMonitor() => _asaServerMonitor.Start();

    public void StopServerMonitor() => _asaServerMonitor.Stop();

    public async Task UpdateServerAsync()
    {
        if (!CanUpdateServer)
            return;

        var arguments = $"+login \"{Defaults.SteamUser}\" +app_update {ASASteamServerID} validate +quit";

        UpdatingServer = true;

        await ExecuteAndWaitForProcessAsync(SteamCmdPath, arguments).ConfigureAwait(false);

        _updateCount++;

        UpdatingServer = false;
    }

    #endregion

    #region Private Methods

    private ProcessMonitor CreateMonitor(string path) =>
        !path.IsNullOrWhiteSpace()
            ? new ProcessMonitor(_fileSystemService.GetFileNameWithoutExtension(path))
            : null;

    private async Task ExecuteAndWaitForProcessAsync(string exe, string arguments = null)
    {
        var process = _processHelper.CreateProcess(exe, arguments);

        process.Start();

        await Task.Run(process.WaitForExit);
    }

    private void ExecuteOpenFolderCommand(ServerFolders folder)
    {
        var (dir, exists) = GetFolder(folder);

        if (!exists)
        {
            var message = dir is { } ? "Cannot find folder!" : $"Cannot find folder \"{dir}\"!";
            _dialogService.ShowErrorMessage(message);
            return;
        }

        _processHelper.CreateProcess("Explorer.exe", dir).Start();
    }

    private void ExecuteOpenArkConfigFileCommand(ArkConfigFiles arkConfigFile)
    {
        string fileName;
        string folder;
        bool createIfNotFound = true;

        switch (arkConfigFile)
        {
            case ArkConfigFiles.GUS:
            case ArkConfigFiles.Game:
                createIfNotFound = false;

                folder = _fileSystemService.Combine(
                    GetFolder(ServerFolders.ServerSave).Folder,
                    "Config",
                    "WindowsServer"
                );

                fileName = arkConfigFile == ArkConfigFiles.GUS ? "GameUserSettings.ini" : "Game.ini";

                break;

            case ArkConfigFiles.AllowedCheatersList:
                (folder, _) = GetFolder(ServerFolders.ServerPath);
                fileName = "AllowedCheaterAccountIDs.txt";
                break;

            case ArkConfigFiles.ExclusiveJoinList:
                (folder, _) = GetFolder(ServerFolders.ServerPath);
                fileName = "PlayersExclusiveJoinList.txt";
                break;

            case ArkConfigFiles.JoinNoCheckList:
                (folder, _) = GetFolder(ServerFolders.ServerPath);
                fileName = "PlayersJoinNoCheckList.txt";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(arkConfigFile), arkConfigFile, null);
        }

        var configFilePath = _fileSystemService.Combine(folder, fileName);

        if (!_fileSystemService.FileExists(configFilePath))
        {
            if (createIfNotFound)
            {
                var result = _dialogService.ShowMessage($"Cannot find \"{fileName}\"!\r\n\r\nWould you like to create it?", "Open config", MessageBoxButton.YesNo);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                _fileSystemService.WriteAllText(configFilePath, string.Empty);
            }
            else
            {
                var message = $"Cannot find \"{fileName}\"!\r\n\r\nPlease ensure your server paths are correct, and that you have run the server at least once!";
                _dialogService.ShowErrorMessage(message);
                return;
            }
        }

        _processHelper.RunWithShellExecute(configFilePath);
    }

    private string GetParentFolder(string path, string relativeTo)
    {
        if (path.IsNullOrWhiteSpace())
            return null;

        var directoryName = _fileSystemService.GetDirectoryName(path);

        return directoryName.IsNullOrWhiteSpace()
            ? relativeTo
            : _fileSystemService.GetFullPath(directoryName, relativeTo);
    }

    private bool IsFileValid(string file) => !file.IsNullOrWhiteSpace() && _fileSystemService.FileExists(file);

    private void OnServerPathChanged()
    {
        DisposeField(ref _serverMonitorSubscription);
        DisposeField(ref _asaServerMonitor);

        _asaServerMonitor = CreateMonitor(ServerPath);

        if (_asaServerMonitor != null)
        {
            _serverMonitorSubscription =
                _asaServerMonitor
                    .FromIsRunningChangedPattern()
                    .Subscribe(_ => UpdateCanStatuses());
        }

        UpdateCanStatuses();
    }

    private void OnSteamCmdPathChanged()
    {
        DisposeField(ref _steamCmdMonitorSubscription);
        DisposeField(ref _steamCmdMonitor);

        _steamCmdMonitor = CreateMonitor(SteamCmdPath);

        if (_asaServerMonitor != null)
        {
            _steamCmdMonitorSubscription =
                _steamCmdMonitor
                    .FromIsRunningChangedPattern()
                    .Subscribe(_ => UpdateCanStatuses());
        }

        RaisePropertyChanged(nameof(SteamCmdPathIsValid));
        UpdateCanStatuses();
    }

    private string ServerFolder() =>
        ServerPath is { } path
            ? _fileSystemService.GetDirectoryName(path)
            : null;

    private void UpdateCanStatuses() =>
        RaisePropertiesChanged(
            nameof(CanRunServer),
            nameof(CanUpdateServer),
            nameof(CanBackupServer)
        );

    #endregion
}