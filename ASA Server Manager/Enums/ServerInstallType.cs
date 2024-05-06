using ASA_Server_Manager.Attributes;

namespace ASA_Server_Manager.Enums;

public enum ServerInstallType
{
    [EnumDisplayValue("SteamCMD")]
    SteamCMD,

    [EnumDisplayValue("Standalone")]
    Standalone,
}