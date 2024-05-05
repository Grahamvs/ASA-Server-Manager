namespace ASA_Server_Manager.Interfaces.Services;

public interface IApplicationService
{
    #region Public Properties

    string Company { get; }

    string Copyright { get; }

    string Description { get; }

    string ExeDirectory { get; }

    string ExeName { get; }

    string ExePath { get; }

    string Name { get; }

    string Title { get; }

    string TradeMark { get; }

    string VersionString { get; }

    string WorkingDirectory { get; }

    #endregion

    #region Public Methods

    void Shutdown();

    #endregion
}