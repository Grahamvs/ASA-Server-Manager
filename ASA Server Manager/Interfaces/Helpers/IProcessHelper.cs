using System.Diagnostics;
using ASA_Server_Manager.Interfaces.Wrappers;

namespace ASA_Server_Manager.Interfaces.Helpers;

public interface IProcessHelper
{
    IProcess CreateProcess();

    IProcess CreateProcess(string fileName, string arguments = null);

    IProcess CreateProcess(Process process);

    IProcess CreateProcess(ProcessStartInfo startInfo);

    IProcess GetProcessById(int id);

    IEnumerable<IProcess> GetProcessByName(string name);

    IEnumerable<IProcess> GetProcesses();

    IEnumerable<IProcess> GetProcessesByName(string name);

    void RunWithShellExecute(string path);
}