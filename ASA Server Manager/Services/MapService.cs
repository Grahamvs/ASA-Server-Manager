using ASA_Server_Manager.Common;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Interfaces.Serialization;
using ASA_Server_Manager.Interfaces.Services;

namespace ASA_Server_Manager.Services;

public class MapService : BindableBase, IMapService
{
    #region Private Fields

    private static readonly IReadOnlyList<MapDetails> OfficialMapIDs =
    [
        new MapDetails {ID = "TheIsland_WP", Name = "The Island"},
        new MapDetails {ID = "ScorchedEarth_WP", Name = "Scorched Earth"},
        new MapDetails {ID = "thecenter_wp", Name = "The Center"},
        new MapDetails {ID = "Aberration_WP", Name = "Aberration"},
    ];

    private readonly IDialogService _dialogService;
    private readonly string _filePath = "AvailableMaps.txt";
    private readonly IFileSystemService _fileSystemService;
    private readonly ISerializer _serializer;
    private List<MapDetails> _customMaps = [];

    #endregion

    #region Public Constructors

    public MapService(
        IFileSystemService fileSystemService,
        ISerializer serializer,
        IDialogService dialogService)
    {
        _fileSystemService = fileSystemService;
        _serializer = serializer;
        _dialogService = dialogService;
    }

    #endregion

    #region Public Properties

    public IReadOnlyList<MapDetails> AvailableMaps =>
        OfficialMaps
            .Concat(CustomMaps.Where(map => OfficialMapIDs.All(officialMap => officialMap.ID != map.ID)))
            .ToList();

    public IReadOnlyList<MapDetails> CustomMaps => _customMaps;

    public IReadOnlyList<MapDetails> OfficialMaps => OfficialMapIDs;

    #endregion

    #region Public Methods

    public void RefreshAvailableMaps()
    {
        _customMaps.Clear();

        List<MapDetails> mapsList = null;
        try
        {
            if (_fileSystemService.FileExists(_filePath))
            {
                mapsList = _serializer.DeserializeFromFile<List<MapDetails>>(_filePath);
            }
        }
        catch (Exception exception)
        {
            _dialogService.ShowErrorMessage($"Unable to load custom maps:\r\n\r\n{exception.Message}");
        }

        SetCustomMaps(mapsList);
    }

    public void Save()
    {
        try
        {
            _serializer.SerializeToFile(_customMaps, _filePath);
        }
        catch (Exception exception)
        {
            _dialogService.ShowErrorMessage($"Unable to save to file:\r\n\r\n{exception.Message}");
        }
    }

    public void SetCustomMaps(IEnumerable<MapDetails> maps)
    {
        maps ??= [];

        var officialIDs = OfficialMapIDs.Select(m => m.ID).ToList();

        _customMaps = maps.Where(map => !officialIDs.Contains(map.ID)).ToList();

        RaisePropertiesChanged(nameof(CustomMaps), nameof(AvailableMaps));
    }

    #endregion
}