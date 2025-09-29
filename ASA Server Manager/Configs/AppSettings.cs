using ASA_Server_Manager.Common;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Interfaces.Configs;
using Range = ASA_Server_Manager.Common.Range;

namespace ASA_Server_Manager.Configs;

public class AppSettings : BindableBase, IAppSettings
{
    #region Private Fields

    private bool _autoSaveProfile = true;
    private string _backupExecutablePath;
    private UpdateFrequency _checkForAppUpdates = UpdateFrequency.OnStart;
    private string _ignoredAppVersion;
    private bool _includePreReleases;
    private DateTime? _lastCheckedForAppUpdate;
    private LinkedList<string> _recentProfiles = new();
    private int _recentProfilesLimit = 5;
    private string _serverPath;
    private ServerInstallType _serverType;
    private bool _showModIDColumn;
    private string _steamCmdPath;
    private bool _updateOnFirstRun = true;

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

    public UpdateFrequency CheckForAppUpdates
    {
        get => _checkForAppUpdates;
        set => SetProperty(ref _checkForAppUpdates, value, OnCheckForAppUpdatesChanged);
    }

    public string IgnoredAppVersion
    {
        get => _ignoredAppVersion;
        set => SetProperty(ref _ignoredAppVersion, value);
    }

    public bool IncludePreReleases
    {
        get => _includePreReleases;
        set => SetProperty(ref _includePreReleases, value);
    }

    public DateTime? LastCheckedForAppUpdate
    {
        get => _lastCheckedForAppUpdate;
        set => SetProperty(ref _lastCheckedForAppUpdate, value);
    }

    public string LastProfile => _recentProfiles.First?.Value;

    IReadOnlyList<string> IAppSettings.RecentProfiles => _recentProfiles.ToList();

    public LinkedList<string> RecentProfiles
    {
        get => _recentProfiles;
        private set => SetProperty(ref _recentProfiles, value);
    }

    public int RecentProfilesLimit
    {
        get => _recentProfilesLimit;
        set => SetProperty(ref _recentProfilesLimit, Range.SetInRange(value, 0, 100), () => TrimRecentProfilesList(true));
    }

    public string ServerPath
    {
        get => _serverPath;
        set => SetProperty(ref _serverPath, value);
    }

    public ServerInstallType ServerType
    {
        get => _serverType;
        set => SetProperty(ref _serverType, value);
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

    #endregion

    #region Public Methods

    public void AddRecentProfile(string path)
    {
        if (_recentProfiles.Contains(path))
        {
            _recentProfiles.Remove(path);
        }

        _recentProfiles.AddFirst(path);

        TrimRecentProfilesList(false);

        RaisePropertiesChanged(nameof(RecentProfiles), nameof(LastProfile));
    }

    #endregion

    #region Private Methods

    private void OnCheckForAppUpdatesChanged()
    {
        if (CheckForAppUpdates == UpdateFrequency.Never)
        {
            LastCheckedForAppUpdate = null;
        }
    }

    private void TrimRecentProfilesList(bool raiseChanged)
    {
        var updated = false;

        while (_recentProfiles.Count > RecentProfilesLimit)
        {
            _recentProfiles.RemoveLast();
            updated = true;
        }

        if (updated && raiseChanged)
        {
            RaisePropertyChanged(nameof(RecentProfiles));
        }
    }

    #endregion
}