using System.ComponentModel;
using ASA_Server_Manager.Interfaces.Configs;

namespace ASA_Server_Manager.Interfaces.Services;

public interface IServerProfileService : INotifyPropertyChanged
{
    IServerProfile CurrentProfile { get; }

    string CurrentFileName { get; }

    string CurrentFilePath { get; }

    bool IsFileLoaded { get; }

    void LoadProfile(string filePath);

    void LoadLastProfile();

    void ResetProfile();

    void SaveProfile(string filePath);
}