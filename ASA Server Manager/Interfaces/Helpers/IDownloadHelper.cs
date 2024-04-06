using System.IO;

namespace ASA_Server_Manager.Interfaces.Helpers;

public interface IDownloadHelper
{
    Task DownloadFileToStream(string url, Stream stream, IProgress<double> progress = null, CancellationToken? cancellationToken = null);
}