using System.IO;
using System.Net.Http;
using ASA_Server_Manager.Interfaces.Helpers;

namespace ASA_Server_Manager.Helpers;

public class DownloadHelper : IDownloadHelper
{
    public async Task DownloadFileToStream(string url, Stream stream, IProgress<double> progress = null, CancellationToken? cancellationToken = null)
    {
        var token = cancellationToken ?? CancellationToken.None;

        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault();
        var contentStream = await response.Content.ReadAsStreamAsync(token);

        // Using a buffer size of 1 MB will help to reduce the number of I/O operations, speeding up
        // the download for large files and reducing the load on the server.
        var buffer = new byte[1048576]; // 1MB
        var totalBytesRead = 0L;

        while (true)
        {
            var bytesRead = await contentStream.ReadAsync(buffer, token);

            if (bytesRead == 0)
            {
                break;
            }

            await stream.WriteAsync(buffer, 0, bytesRead, token);
            totalBytesRead += bytesRead;

            if (progress == null)
                continue;

            var percentage = (double)totalBytesRead / totalBytes * 100;
            await Task.Run(() => progress.Report(percentage), token).ConfigureAwait(false);
        }
    }
}