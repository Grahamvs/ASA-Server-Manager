namespace ASA_Server_Manager.Interfaces.Services;

public interface IApplicationService
{
    string Company { get; }

    string Copyright { get; }

    string Description { get; }

    string ExeDirectory { get; }

    string ExeName { get; }

    string ExePath { get; }

    string Title { get; }

    string TradeMark { get; }

    string VersionString { get; }

    string WorkingDirectory { get; }

    void Shutdown();
}