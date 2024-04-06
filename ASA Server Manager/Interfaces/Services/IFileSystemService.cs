namespace ASA_Server_Manager.Interfaces.Services;

public interface IFileSystemService
{
    #region Public Methods

    string Combine(params string[] paths);

    void DeleteFile(string filePath);

    bool DirectoryExists(string path);

    bool FileExists(string fileName);

    string GetDirectoryName(string filePath);

    string GetFileName(string path);

    string GetFileNameWithoutExtension(string path);

    string[] GetFiles(string path, string searchPattern = null, bool includeSubDirectories = false);

    string GetFullPath(string path);

    string GetFullPath(string path, string basePath);

    string GetRelativePath(string relativeTo, string path);

    bool IsPathRooted(string path);

    IEnumerable<string> ReadAllLines(string fileName);

    string ReadAllText(string path);

    void WriteAllLines(
        string fileName,
        IEnumerable<string> contents
    );

    void WriteAllText(
        string fileName,
        string text
    );

    #endregion
}