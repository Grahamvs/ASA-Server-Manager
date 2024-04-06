using System.Diagnostics;
using ASA_Server_Manager.Interfaces.Helpers;
using ASA_Server_Manager.Interfaces.Wrappers;
using ASA_Server_Manager.Wrappers;

namespace ASA_Server_Manager.Helpers;

public class ProcessHelper : IProcessHelper
{
    public IProcess CreateProcess() => new ProcessWrapper();

    public IProcess CreateProcess(string fileName, string arguments = null) => new ProcessWrapper(fileName, arguments);

    public IProcess CreateProcess(Process process) => new ProcessWrapper(process);

    public IProcess CreateProcess(ProcessStartInfo startInfo) => new ProcessWrapper(startInfo);

    public IProcess GetProcessById(int id) => new ProcessWrapper(Process.GetProcessById(id));

    public IEnumerable<IProcess> GetProcessByName(string name) => Process.GetProcessesByName(name).Select(process => new ProcessWrapper(process));

    public IEnumerable<IProcess> GetProcesses() => Process.GetProcesses().Select(p => new ProcessWrapper(p));

    public IEnumerable<IProcess> GetProcessesByName(string name) => Process.GetProcessesByName(name).Select(p => new ProcessWrapper(p));
}