using ASA_Server_Manager.Common;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Services;

namespace ASA_Server_Manager.Services;

public class MapService : BindableBase, IMapService
{
    #region Private Fields

    private static readonly IReadOnlyList<string> OfficialMapIDs = ["TheIsland_WP", "ScorchedEarth_WP"];

    private readonly List<string> _customIDs = [];
    private readonly string _filePath = "AvailableMaps.txt";
    private readonly IFileSystemService _fileSystemService;

    #endregion

    #region Public Constructors

    public MapService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
    }

    #endregion

    #region Public Properties

    public IReadOnlyList<string> AvailableIDs =>
        OfficialIDs
            .Union(_customIDs, StringComparer.OrdinalIgnoreCase)
            .ToList();

    public IReadOnlyList<string> CustomIDs => _customIDs;

    public IReadOnlyList<string> OfficialIDs => OfficialMapIDs;

    #endregion

    #region Public Methods

    public void RefreshAvailableMaps()
    {
        _customIDs.Clear();

        if (EnsureMapIDFileExists())
        {
            _customIDs.AddRange(
                _fileSystemService.ReadAllLines(_filePath)
                    .Where(value => !value.IsNullOrWhiteSpace())
                    .Select(value => value?.Trim())
                    .Except(OfficialMapIDs, StringComparer.OrdinalIgnoreCase)
            );
        }

        RaisePropertyChanged(nameof(CustomIDs));
        RaisePropertyChanged(nameof(AvailableIDs));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Ensures the map ID file list exists.
    /// </summary>
    /// <returns> Returns true if the file already exists </returns>
    private bool EnsureMapIDFileExists()
    {
        if (_fileSystemService.FileExists(_filePath))
            return true;

        _fileSystemService.WriteAllLines(_filePath, AvailableIDs);

        return false;
    }

    #endregion
}