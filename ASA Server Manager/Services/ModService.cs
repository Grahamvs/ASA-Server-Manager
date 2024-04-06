using ASA_Server_Manager.Common;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Interfaces.Serialization;
using ASA_Server_Manager.Interfaces.Services;

namespace ASA_Server_Manager.Services;

public class ModService : BindableBase, IModService
{
    #region Private Fields

    private readonly List<Mod> _availableModsList = [];
    private readonly string _filePath;
    private readonly IFileSystemService _fileSystemService;
    private readonly ISerializer _serializer;

    #endregion

    #region Public Constructors

    public ModService(
        IApplicationService applicationService,
        IFileSystemService fileSystemService,
        ISerializer serializer
    )
    {
        _fileSystemService = fileSystemService;
        _serializer = serializer;

        _filePath = $"{applicationService.ExeName}.mods";
    }

    #endregion

    #region Public Properties

    public IEnumerable<Mod> AvailableModsList => _availableModsList;

    #endregion

    #region Public Methods

    public void Load()
    {
        _availableModsList.Clear();
        IEnumerable<Mod> loadedMods = null;

        if (_fileSystemService.FileExists(_filePath))
        {
            var serializedMods = _fileSystemService.ReadAllText(_filePath);
            loadedMods = _serializer.Deserialize<IEnumerable<Mod>>(serializedMods);
        }

        SetMods(loadedMods);
    }

    public void Save()
    {
        var serializedMods = _serializer.Serialize(_availableModsList);
        _fileSystemService.WriteAllText(_filePath, serializedMods);
    }

    public void SetMods(IEnumerable<Mod> mods)
    {
        _availableModsList.Clear();

        var modList = mods as IList<Mod> ?? mods?.ToList() ?? [];

        if (modList.Count > 0)
        {
            _availableModsList.AddRange(modList);
        }
    }

    #endregion
}