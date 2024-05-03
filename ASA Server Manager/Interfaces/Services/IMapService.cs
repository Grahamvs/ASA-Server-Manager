using System.ComponentModel;
using ASA_Server_Manager.Configs;

namespace ASA_Server_Manager.Interfaces.Services;

public interface IMapService : INotifyPropertyChanged
{
    #region Public Properties

    IReadOnlyList<MapDetails> AvailableMaps { get; }

    IReadOnlyList<MapDetails> CustomMaps { get; }

    IReadOnlyList<MapDetails> OfficialMaps { get; }

    #endregion

    #region Public Methods

    void RefreshAvailableMaps();

    void Save();

    void SetCustomMaps(IEnumerable<MapDetails> maps);

    #endregion
}