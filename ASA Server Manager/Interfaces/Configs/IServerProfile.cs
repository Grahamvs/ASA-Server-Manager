using System.ComponentModel;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Interfaces.Common;

namespace ASA_Server_Manager.Interfaces.Configs;

public interface IServerProfile : INotifyPropertyChanged, IHasChanges
{
    #region Public Properties

    string AdditionalArguments { get; set; }

    string AdminPassword { get; set; }

    bool AllowCryoFridgeOnSaddle { get; set; }

    string ClusterDirOverride { get; set; }

    string ClusterID { get; set; }

    bool IsValid { get; }

    string MapID { get; set; }

    int? MaxPlayers { get; set; }

    int? MaxTributeDinos { get; set; }

    int? MaxTributeItems { get; set; }

    bool NoTransferFromFiltering { get; set; }

    bool NoTributeDownloads { get; set; }

    bool NoWildBabies { get; set; }

    double? PlatformMaxStructuresMultiplier { get; set; }

    int? Port { get; set; }

    bool PreventDownloadDinos { get; set; }

    bool PreventDownloadItems { get; set; }

    bool PreventDownloadSurvivors { get; set; }

    bool PreventSpawnAnimations { get; set; }

    int? QueryPort { get; set; }

    bool RandomSupplyCratePoints { get; set; }

    bool RCONEnabled { get; set; }

    int? RCONPort { get; set; }

    int? RCONServerGameLogBuffer { get; set; }

    string SaveDirectoryName { get; set; }

    IReadOnlyDictionary<int, ModMode> SelectedMods { get; }

    bool ServerGameLog { get; set; }

    bool ServerGameLogIncludeTribeLogs { get; set; }

    string ServerPassword { get; set; }

    public bool ServerRCONOutputTribeLogs { get; set; }

    string SessionName { get; set; }

    bool UseBattlEye { get; set; }

    #endregion

    #region Public Methods

    void SetModMode(int modID, ModMode mode);

    #endregion
}