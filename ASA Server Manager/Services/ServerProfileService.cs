using System.IO;
using ASA_Server_Manager.Common;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Configs;
using ASA_Server_Manager.Interfaces.Serialization;
using ASA_Server_Manager.Interfaces.Services;

namespace ASA_Server_Manager.Services;

public class ServerProfileService : BindableBase, IServerProfileService
{
    #region Private Fields

    private readonly string _defaultFileName = $"New Profile.{ProfileExtension}";

    private readonly IFileSystemService _fileSystemService;
    private readonly IApplicationService _applicationService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly IDialogService _dialogService;
    private readonly IMapService _mapService;
    private readonly ISerializer _serializer;
    private ServerProfile _currentProfile;
    private string _currentFilePath;

    #endregion

    #region Public Constructors

    public ServerProfileService(
        IApplicationService applicationService,
        IAppSettingsService appSettingsService,
        IDialogService dialogService,
        IMapService mapService,
        IFileSystemService fileSystemService,
        ISerializer serializer
    )
    {
        _applicationService = applicationService;
        _appSettingsService = appSettingsService;
        _dialogService = dialogService;
        _mapService = mapService;
        _fileSystemService = fileSystemService;
        _serializer = serializer;
    }

    #endregion

    #region Public Properties

    public static string ProfileDescription => "Server Profile";

    public static string ProfileExtension => "profile";

    public IServerProfile CurrentProfile => _currentProfile ??= CreateDefaultProfile();

    public string CurrentFileName => _fileSystemService.GetFileNameWithoutExtension(CurrentFilePath.IsNullOrWhiteSpace() ? _defaultFileName : CurrentFilePath);

    public string CurrentFilePath
    {
        get => _currentFilePath;
        private set => SetProperty(ref _currentFilePath, value, OnCurrentFilePathChanged);
    }

    private void OnCurrentFilePathChanged()
    {
        _appSettingsService.AddRecentProfile(_fileSystemService.GetRelativePath(_applicationService.WorkingDirectory, CurrentFilePath));

        RaisePropertiesChanged(
            nameof(CurrentFileName),
            nameof(IsFileLoaded)
        );
    }

    public bool IsFileLoaded => !CurrentFilePath.IsNullOrWhiteSpace();

    #endregion

    #region Public Methods

    public ServerProfile CreateDefaultProfile() =>
        new()
        {
            MapID = _mapService.OfficialMaps[0].ID,
            Port = Defaults.Port,
            AllowCryoFridgeOnSaddle = false,
            PreventSpawnAnimations = false,
            RandomSupplyCratePoints = false,
            RCONEnabled = false,
            RCONServerGameLogBuffer = Defaults.RCONServerGameLogBuffer,
            ServerGameLog = false,
            ServerRCONOutputTribeLogs = false,
            UseBattlEye = false,
        };

    public void LoadProfile(string filePath) => LoadProfile(filePath, true);

    private void LoadProfile(string filePath, bool saveOnLoad)
    {
        try
        {
            if (_fileSystemService.FileExists(filePath))
            {
                _currentProfile = _serializer.DeserializeFromFile<ServerProfile>(filePath);

                _currentProfile.ResetHasChanges();

                CurrentFilePath = filePath;

                if (saveOnLoad)
                {
                    _appSettingsService.SaveSettings();
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
        catch (Exception exception)
        {
            _dialogService.ShowErrorMessage($"Error loading {ProfileDescription}!\r\n\r\n{exception.Message}");
        }

        RaisePropertyChanged(nameof(CurrentProfile));
    }

    public void LoadLastProfile()
    {
        if (_appSettingsService.LastProfile != null)
        {
            LoadProfile(_appSettingsService.LastProfile, false);
        }
        else
        {
            ResetProfile();
        }
    }

    public void ResetProfile()
    {
        _currentProfile = CreateDefaultProfile();
        CurrentFilePath = null;

        RaisePropertyChanged(nameof(CurrentProfile));
    }

    public void SaveProfile(string filePath)
    {
        try
        {
            var path = filePath ?? throw new ArgumentNullException(nameof(filePath));

            _serializer.SerializeToFile(_currentProfile, path);

            _currentProfile.ResetHasChanges();

            CurrentFilePath = filePath;
        }
        catch (Exception exception)
        {
            _dialogService.ShowErrorMessage($"Error saving {ProfileDescription}!\r\n\r\n{exception.Message}");
        }
    }

    #endregion
}