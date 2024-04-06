using System.ComponentModel;
using ASA_Server_Manager.Configs;

namespace ASA_Server_Manager.Interfaces.Services;

public interface IModService : INotifyPropertyChanged
{
    IEnumerable<Mod> AvailableModsList { get; }

    void Load();

    void Save();

    void SetMods(IEnumerable<Mod> mods);
}