using ASA_Server_Manager.Interfaces.Configs;

namespace ASA_Server_Manager.Interfaces.Services;

public interface IAppSettingsService : IAppSettings
{
    #region Public Methods

    bool LoadSettings();

    void SaveSettings();

    #endregion
}