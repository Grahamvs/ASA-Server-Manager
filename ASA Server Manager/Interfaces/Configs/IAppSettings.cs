using System.ComponentModel;
using ASA_Server_Manager.Enums;

namespace ASA_Server_Manager.Interfaces.Configs;

public interface IAppSettings : INotifyPropertyChanged
{
    #region Public Properties

    bool AutoSaveProfile { get; set; }

    string BackupExecutablePath { get; set; }

    UpdateFrequency CheckForAppUpdates { get; set; }

    string IgnoredAppVersion { get; set; }

    bool IncludePreReleases { get; set; }

    DateTime? LastCheckedForAppUpdate { get; set; }

    string LastProfile { get; }

    IReadOnlyList<string> RecentProfiles { get; }

    int RecentProfilesLimit { get; set; }

    string ServerPath { get; set; }

    ServerInstallType ServerType { get; set; }

    bool ShowModIDColumn { get; set; }

    string SteamCmdPath { get; set; }

    bool UpdateOnFirstRun { get; set; }

    #endregion

    #region Public Methods

    void AddRecentProfile(string path);

    #endregion
}