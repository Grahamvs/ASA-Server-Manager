using ASA_Server_Manager.Attributes;
using ASA_Server_Manager.Common;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Configs;
using Range = ASA_Server_Manager.Common.Range;

namespace ASA_Server_Manager.Configs;

public class ServerProfile : BindableBase, IServerProfile
{
    #region Private Fields

    private string _additionalArguments;
    private string _adminPassword;
    private bool _allowCryoFridgeOnSaddle;
    private string _clusterDirOverride;
    private string _clusterID;
    private bool _convertToStore;
    private bool _exclusiveJoin;
    private bool _hasChanges;
    private bool _isValid;
    private string _mapID;
    private int? _maxPlayers;
    private int? _maxTributeDinos;
    private int? _maxTributeItems;
    private bool _noTransferFromFiltering;
    private bool _noTributeDownloads;
    private bool _noWildBabies;
    private double? _platformMaxStructuresMultiplier;
    private int? _port;
    private bool _preventDownloadDinos;
    private bool _preventDownloadItems;
    private bool _preventDownloadSurvivors;
    private bool _preventSpawnAnimations;
    private int? _queryPort;
    private bool _randomSupplyCratePoints;
    private bool _rconEnabled;
    private int? _rconPort;
    private int? _rconServerGameLogBuffer;
    private string _saveDirectoryName;
    private Dictionary<int, ModMode> _selectedModIds = new();
    private bool _serverGameLog;
    private bool _serverGameLogIncludeTribeLogs;
    private string _serverPassword;
    private bool _serverRconOutputTribeLogs;
    private string _sessionName;
    private bool _useBattlEye;
    private bool _useStore;

    #endregion

    #region Public Properties

    public string AdditionalArguments
    {
        get => _additionalArguments;
        set => SetProperty(ref _additionalArguments, value);
    }

    public string AdminPassword
    {
        get => _adminPassword;
        set => SetProperty(ref _adminPassword, value, Validate);
    }

    public bool AllowCryoFridgeOnSaddle
    {
        get => _allowCryoFridgeOnSaddle;
        set => SetProperty(ref _allowCryoFridgeOnSaddle, value);
    }

    public string ClusterDirOverride
    {
        get => _clusterDirOverride;
        set => SetProperty(ref _clusterDirOverride, value);
    }

    public string ClusterID
    {
        get => _clusterID;
        set => SetProperty(ref _clusterID, value);
    }

    public bool ConvertToStore
    {
        get => _convertToStore;
        set => SetProperty(ref _convertToStore, value);
    }

    public bool ExclusiveJoin
    {
        get => _exclusiveJoin;
        set => SetProperty(ref _exclusiveJoin, value);
    }

    [DoNotSerialize]
    public bool HasChanges
    {
        get => _hasChanges;
        private set => SetProperty(ref _hasChanges, value);
    }

    [DoNotSerialize]
    public bool IsValid
    {
        get => _isValid;
        private set => SetProperty(ref _isValid, value);
    }

    public string MapID
    {
        get => _mapID;
        set => SetProperty(ref _mapID, value, Validate);
    }

    public int? MaxPlayers
    {
        get => _maxPlayers;
        set => SetProperty(ref _maxPlayers, value);
    }

    public int? MaxTributeDinos
    {
        get => _maxTributeDinos;
        set => SetProperty(ref _maxTributeDinos, Range.SetInRange(value, 0, 150));
    }

    public int? MaxTributeItems
    {
        get => _maxTributeItems;
        set => SetProperty(ref _maxTributeItems, Range.SetInRange(value, 0, 250));
    }

    public bool NoTransferFromFiltering
    {
        get => _noTransferFromFiltering;
        set => SetProperty(ref _noTransferFromFiltering, value);
    }

    public bool NoTributeDownloads
    {
        get => _noTributeDownloads;
        set => SetProperty(ref _noTributeDownloads, value);
    }

    public bool NoWildBabies
    {
        get => _noWildBabies;
        set => SetProperty(ref _noWildBabies, value);
    }

    public double? PlatformMaxStructuresMultiplier
    {
        get => _platformMaxStructuresMultiplier;
        set => SetProperty(ref _platformMaxStructuresMultiplier, value);
    }

    public int? Port
    {
        get => _port;
        set => SetProperty(ref _port, Range.SetInRange(value, Defaults.MinServerPort, Defaults.MaxServerPort));
    }

    public bool PreventDownloadDinos
    {
        get => _preventDownloadDinos;
        set => SetProperty(ref _preventDownloadDinos, value);
    }

    public bool PreventDownloadItems
    {
        get => _preventDownloadItems;
        set => SetProperty(ref _preventDownloadItems, value);
    }

    public bool PreventDownloadSurvivors
    {
        get => _preventDownloadSurvivors;
        set => SetProperty(ref _preventDownloadSurvivors, value);
    }

    public bool PreventSpawnAnimations
    {
        get => _preventSpawnAnimations;
        set => SetProperty(ref _preventSpawnAnimations, value);
    }

    public int? QueryPort
    {
        get => _queryPort;
        set => SetProperty(ref _queryPort, Range.SetInRange(value, Defaults.MinServerPort, Defaults.MaxServerPort));
    }

    public bool RandomSupplyCratePoints
    {
        get => _randomSupplyCratePoints;
        set => SetProperty(ref _randomSupplyCratePoints, value);
    }

    public bool RCONEnabled
    {
        get => _rconEnabled;
        set => SetProperty(ref _rconEnabled, value);
    }

    public int? RCONPort
    {
        get => _rconPort;
        set => SetProperty(ref _rconPort, Range.SetInRange(value, Defaults.MinServerPort, Defaults.MaxServerPort));
    }

    public int? RCONServerGameLogBuffer
    {
        get => _rconServerGameLogBuffer;
        set => SetProperty(ref _rconServerGameLogBuffer, Range.SetInRange(value, Defaults.RCONServerGameLogBufferMin, Defaults.RCONServerGameLogBufferMax));
    }

    public string SaveDirectoryName
    {
        get => _saveDirectoryName;
        set => SetProperty(ref _saveDirectoryName, value);
    }

    public Dictionary<int, ModMode> SelectedModIds
    {
        get => _selectedModIds;
        set => _selectedModIds = value ?? new Dictionary<int, ModMode>();
    }

    IReadOnlyDictionary<int, ModMode> IServerProfile.SelectedMods => _selectedModIds;

    public bool ServerGameLog
    {
        get => _serverGameLog;
        set => SetProperty(ref _serverGameLog, value);
    }

    public bool ServerGameLogIncludeTribeLogs
    {
        get => _serverGameLogIncludeTribeLogs;
        set => SetProperty(ref _serverGameLogIncludeTribeLogs, value);
    }

    public string ServerPassword
    {
        get => _serverPassword;
        set => SetProperty(ref _serverPassword, value, Validate);
    }

    public bool ServerRCONOutputTribeLogs
    {
        get => _serverRconOutputTribeLogs;
        set => SetProperty(ref _serverRconOutputTribeLogs, value);
    }

    public string SessionName
    {
        get => _sessionName;
        set => SetProperty(ref _sessionName, value, Validate);
    }

    public bool UseBattlEye
    {
        get => _useBattlEye;
        set => SetProperty(ref _useBattlEye, value);
    }

    public bool UseStore
    {
        get => _useStore;
        set => SetProperty(ref _useStore, value);
    }

    #endregion

    #region Public Methods

    public void ResetHasChanges() => HasChanges = false;

    public void SetModMode(int modID, ModMode mode)
    {
        switch (mode)
        {
            case ModMode.Disabled:
                HasChanges |= _selectedModIds.Remove(modID);
                break;

            case ModMode.Enabled:
            case ModMode.Passive:
                if (!_selectedModIds.TryGetValue(modID, out var val) || val != mode)
                {
                    HasChanges = true;
                    _selectedModIds[modID] = mode;
                }

                break;

            default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    #endregion

    #region Protected Methods

    protected override void OnRaisePropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(IsValid):
            case nameof(HasChanges):

                // Ignore
                break;

            default:
                HasChanges = true;
                break;
        }
    }

    #endregion

    #region Private Methods

    private void Validate()
    {
        IsValid = !MapID.IsNullOrEmpty() && !SessionName.IsNullOrWhiteSpace();
    }

    #endregion
}