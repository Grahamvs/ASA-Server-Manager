using System.ComponentModel;

namespace ASA_Server_Manager.Interfaces.Services;

public interface IMapService : INotifyPropertyChanged
{
    IReadOnlyList<string> AvailableIDs { get; }

    IReadOnlyList<string> CustomIDs { get; }

    IReadOnlyList<string> OfficialIDs { get; }

    void RefreshAvailableMaps();
}