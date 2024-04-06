namespace ASA_Server_Manager.Configs;

public static class Defaults
{
    #region Public Properties

    public static string ASARelativePath => "steamapps\\common\\ARK Survival Ascended Dedicated Server\\ShooterGame\\Binaries\\Win64\\ArkAscendedServer.exe";

    public static int MaxPlayers => 70;

    public static int MaxPlayersMax => 200;

    public static int MaxPlayersMin => 1;

    public static int MaxServerPort => 49151;

    public static int MaxTributeDinos => 20;

    public static int MaxTributeDinosMax => 150;

    public static int MaxTributeDinosMin => 20;

    public static int MaxTributeItems => 50;

    public static int MaxTributeItemsMax => 250;

    public static int MaxTributeItemsMin => 50;

    public static int MinServerPort => 1024;

    public static double PlatformMaxStructuresMultiplier => 1.0;

    public static double PlatformMaxStructuresMultiplierMax => 100.0;

    public static double PlatformMaxStructuresMultiplierMin => 0;

    public static int Port => 7777;

    public static int QueryPort => 27015;

    public static int RCONPort => 27020;

    public static int RCONServerGameLogBuffer => 600;

    public static int RCONServerGameLogBufferMax => 9999;

    public static int RCONServerGameLogBufferMin => 0;

    public static int RecentProfilesLimit => 5;

    public static string SaveFolder => "SaveGames";

    public static string SteamUser => "anonymous";

    #endregion
}