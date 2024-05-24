using System.ComponentModel;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Interfaces.Common.Commands;
using ASA_Server_Manager.Interfaces.Configs;

namespace ASA_Server_Manager.Interfaces.Helpers;

public interface IServerHelper : INotifyPropertyChanged
{
    #region Public Properties

    bool BackupExecutablePathIsValid { get; }

    bool CanBackupServer { get; }

    bool CanRunServer { get; }

    bool CanUpdateServer { get; }

    ICommand<ServerFolders> OpenFolderCommand { get; }

    bool SteamCmdPathIsValid { get; }

    bool UpdatingServer { get; }

    #endregion

    #region Public Methods

    Task<string> DownloadSteamCmdAsync(string steamCmdFolder, IProgress<double> progress = null, CancellationToken? cancellationToken = null);

    (string Folder, bool Exists) GetFolder(ServerFolders folder);

    Task RunBackupExecutable();

    Task RunServerAsync(IServerProfile profile);

    void StartServerMonitor();

    void StopServerMonitor();

    Task UpdateServerAsync();

    #endregion
}