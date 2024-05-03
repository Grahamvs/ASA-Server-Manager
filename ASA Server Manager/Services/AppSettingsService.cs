using System.ComponentModel;
using ASA_Server_Manager.Common;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Configs;
using ASA_Server_Manager.Interfaces.Serialization;
using ASA_Server_Manager.Interfaces.Services;

namespace ASA_Server_Manager.Services;

public class AppSettingsService : BindableBase, IAppSettingsService
{
    #region Private Fields

    private readonly HashSet<string> _appSettingsProperties;
    private readonly string _filePath;
    private readonly IFileSystemService _fileSystemService;
    private readonly ISerializer _serializer;
    private AppSettings _appSettings;
    private IDisposable _appSettingsSub;

    #endregion

    #region Public Constructors

    public AppSettingsService(
        IApplicationService applicationService,
        ISerializer serializer,
        IFileSystemService fileSystemService
    )
    {
        _serializer = serializer;
        _fileSystemService = fileSystemService;

        _filePath = $"{fileSystemService.Combine(applicationService.WorkingDirectory, applicationService.ExeName)}.config";

        _appSettings = new AppSettings();
        _appSettingsProperties = [.. typeof(IAppSettings).GetProperties().Select(p => p.Name)];
    }

    #endregion

    #region Public Properties

    public bool AutoSaveProfile
    {
        get => AppSettings.AutoSaveProfile;
        set => AppSettings.AutoSaveProfile = value;
    }

    public string BackupExecutablePath
    {
        get => AppSettings.BackupExecutablePath;
        set => AppSettings.BackupExecutablePath = value;
    }

    public string LastProfile => AppSettings.LastProfile;

    public IReadOnlyList<string> RecentProfiles => ((IAppSettings)_appSettings).RecentProfiles;

    public int RecentProfilesLimit
    {
        get => _appSettings.RecentProfilesLimit;
        set => _appSettings.RecentProfilesLimit = value;
    }

    public string ServerPath
    {
        get => AppSettings.ServerPath;
        set => AppSettings.ServerPath = value;
    }

    public ServerInstallType ServerType
    {
        get => _appSettings.ServerType;
        set => _appSettings.ServerType = value;
    }

    public bool ShowModIDColumn
    {
        get => _appSettings.ShowModIDColumn;
        set => _appSettings.ShowModIDColumn = value;
    }

    public string SteamCmdPath
    {
        get => AppSettings.SteamCmdPath;
        set => AppSettings.SteamCmdPath = value;
    }

    public bool UpdateOnFirstRun
    {
        get => AppSettings.UpdateOnFirstRun;
        set => AppSettings.UpdateOnFirstRun = value;
    }

    #endregion

    #region Private Properties

    private AppSettings AppSettings
    {
        get => _appSettings;
        set => SetPrivateProperty(ref _appSettings, value, OnAppSettingsChanged);
    }

    #endregion

    #region Public Methods

    public void AddRecentProfile(string path)
    {
        _appSettings.AddRecentProfile(path);
        SaveSettings();
    }

    public bool LoadSettings()
    {
        AppSettings newSettings = null;

        if (_fileSystemService.FileExists(_filePath))
        {
            var text = _fileSystemService.ReadAllText(_filePath);
            newSettings = _serializer.Deserialize<AppSettings>(text);
        }

        AppSettings = newSettings ?? new AppSettings();

        return newSettings != null;
    }

    public void SaveSettings() => _serializer.SerializeToFile(AppSettings, _filePath);

    #endregion

    #region Private Methods

    private void OnAppSettings_PropertyChanged(PropertyChangedEventArgs eventArgs)
    {
        var propertyName = eventArgs.PropertyName;

        if (_appSettingsProperties.Contains(propertyName))
        {
            RaisePropertyChanged(propertyName);
        }
    }

    private void OnAppSettingsChanged()
    {
        DisposeField(ref _appSettingsSub);

        if (AppSettings == null)
            return;

        _appSettingsSub = AppSettings
            .FromPropertyChangedPattern()
            .Subscribe(pattern => OnAppSettings_PropertyChanged(pattern.EventArgs));

        RaisePropertiesChanged(_appSettingsProperties.ToArray());
    }

    #endregion
}