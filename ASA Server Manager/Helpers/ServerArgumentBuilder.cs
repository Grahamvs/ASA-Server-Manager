using System.Text;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Encryption;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Configs;

namespace ASA_Server_Manager.Helpers;

public class ServerArgumentBuilder
{
    #region Private Fields

    private readonly List<string> _optionsList = new();
    private readonly List<string> _settingsList = new();
    private StringBuilder _argumentBuilder;

    #endregion

    #region Public Methods

    public string Build(IServerProfile profile, IEnumerable<Mod> availableModsList)
    {
        _optionsList.Clear();
        _settingsList.Clear();

        var serverPassword = StringEncryptor.Decrypt(profile.ServerPassword);
        var adminPassword = StringEncryptor.Decrypt(profile.AdminPassword);

        // Server Settings
        AddSetting("ServerPassword", serverPassword);
        AddSetting("AltSaveDirectoryName", profile.SaveDirectoryName);
        AddSetting("ServerAdminPassword", adminPassword);
        AddOption(profile.MaxPlayers is > 0, "WinLiveMaxPlayers", profile.MaxPlayers);
        AddOption(profile.ExclusiveJoin, "ExclusiveJoin");

        // RCON Settings
        AddSetting("RCONEnabled", profile.RCONEnabled);
        if (profile.RCONEnabled)
        {
            AddSetting(profile.RCONPort.HasValue, "RCONPort", profile.RCONPort);
            AddSetting(profile.RCONServerGameLogBuffer.HasValue, "RCONServerGameLogBuffer", profile.RCONServerGameLogBuffer);
            AddOption(profile.ServerRCONOutputTribeLogs, "ServerRCONOutputTribeLogs");
        }

        // Cluster & Transfer Settings
        AddOption("ClusterID", profile.ClusterID);
        AddOption("ClusterDirOverride", $"\"{profile.ClusterDirOverride?.Trim('\"')}\"".Trim('\"'));
        AddOption("MaxTributeDinos", profile.MaxTributeDinos);
        AddOption("MaxTributeItems", profile.MaxTributeItems);
        AddOption(profile.NoTransferFromFiltering, "NoTransferFromFiltering");
        AddOption(profile.NoTributeDownloads, "NoTributeDownloads");
        AddOption(profile.PreventDownloadDinos, "PreventDownloadDinos");
        AddOption(profile.PreventDownloadItems, "PreventDownloadItems");
        AddOption(profile.PreventDownloadSurvivors, "PreventDownloadSurvivors");

        // Advanced Settings
        AddOption("Port", profile.Port);
        AddOption("QueryPort", profile.QueryPort);
        AddOption("PerPlatformMaxStructuresMultiplier", profile.PlatformMaxStructuresMultiplier);
        AddOption(profile.AllowCryoFridgeOnSaddle, "AllowCryoFridgeOnSaddle");
        AddOption(profile.PreventSpawnAnimations, "PreventSpawnAnimations");
        AddOption(profile.RandomSupplyCratePoints, "RandomSupplyCratePoints");
        AddOption(profile.NoWildBabies, "NoWildBabies");
        AddOption(!profile.UseBattlEye, "NoBattlEye");
        AddOption(profile.ServerGameLog, "ServerGameLog");
        AddOption(profile.ServerGameLogIncludeTribeLogs, "ServerGameLogIncludeTribeLogs");

        var availableModIDs = availableModsList?.Select(mod => mod.ID).ToList() ?? [];

        // Enabled Mods
        if (profile.SelectedMods.Any())
        {
            var passiveMods = availableModIDs
                .Intersect(
                    profile
                        .SelectedMods
                        .Where(kvp => kvp.Value == ModMode.Passive)
                        .Select(kvp => kvp.Key)
                )
                .ToList();

            var enabledMods = availableModIDs
                .Intersect(
                    profile
                        .SelectedMods
                        .Where(kvp => kvp.Value == ModMode.Enabled)
                        .Select(kvp => kvp.Key)
                )
                .ToList();

            if (passiveMods.Count != 0)
            {
                AddOption(true, "passivemods ", string.Join(",", passiveMods));
            }

            if (enabledMods.Count != 0 || passiveMods.Count != 0)
            {
                // Ensure that any passive mods are included and loaded first!
                AddOption(true, "mods", string.Join(",", passiveMods.Concat(enabledMods)));
            }
        }

        if (!profile.AdditionalArguments.IsNullOrWhiteSpace())
        {
            _optionsList.Add($" {profile.AdditionalArguments.Trim()}");
        }

        _argumentBuilder = new StringBuilder($"{profile.MapID}?SessionName=\"{profile.SessionName}\"");
        _argumentBuilder.Append(string.Join("", _settingsList));
        _argumentBuilder.Append(string.Join("", _optionsList));

        return _argumentBuilder.ToString();
    }

    #endregion

    #region Private Methods

    private void AddOption(bool condition, string option)
    {
        if (condition)
        {
            _optionsList.Add($" -{option}");
        }
    }

    private void AddOption<T>(string option, T? value) => AddOption(value is not null, option, value);

    private void AddOption(string option, string value) => AddOption(!value.IsNullOrWhiteSpace(), option, value);

    private void AddOption<T>(bool condition, string option, T value)
    {
        if (condition && value != null)
        {
            _optionsList.Add($" -{option}={value}");
        }
    }

    private void AddSetting(string setting, string value) => AddSetting(!value.IsNullOrWhiteSpace(), setting, value);

    private void AddSetting<T>(string setting, T value) => AddSetting(value is not null, setting, value?.ToString());

    private void AddSetting<T>(bool condition, string setting, T value) => AddSetting(condition && value != null, setting, value?.ToString());

    private void AddSetting(bool condition, string setting, string value)
    {
        if (condition)
        {
            _settingsList.Add($"?{setting}=\"{value}\"");
        }
    }

    #endregion
}