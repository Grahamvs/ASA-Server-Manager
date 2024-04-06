using System.IO;
using ASA_Server_Manager.Interfaces.Services;

namespace ASA_Server_Manager.Services;

public class FileSystemService : IFileSystemService
{
    #region Public Methods

    public string Combine(params string[] paths) => Path.Combine(paths);

    public void DeleteFile(string filePath) => File.Delete(filePath);

    public bool DirectoryExists(string path) => Directory.Exists(path);

    public bool FileExists(string fileName) => File.Exists(fileName);

    public string GetDirectoryName(string filePath) => Path.GetDirectoryName(filePath);

    public string GetFileName(string path) => Path.GetFileName(path);

    public string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);

    public string[] GetFiles(string path, string searchPattern = null, bool includeSubdirectories = false) => Directory.GetFiles(path, searchPattern ?? "*", includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    public string GetFullPath(string path) => Path.GetFullPath(path);

    public string GetFullPath(string path, string basePath) => Path.GetFullPath(path, basePath);

    public string GetRelativePath(string relativeTo, string path) => Path.GetRelativePath(relativeTo, path);

    public bool IsPathRooted(string path) => Path.IsPathRooted(path);

    public IEnumerable<string> ReadAllLines(string fileName) => File.ReadAllLines(fileName);

    public string ReadAllText(string path) => File.ReadAllText(path);

    public void WriteAllLines(
        string fileName,
        IEnumerable<string> contents
    ) =>
        File.WriteAllLines(fileName, contents);

    public void WriteAllText(
        string fileName,
        string text
    ) =>
        File.WriteAllText(fileName, text);

    #endregion
}