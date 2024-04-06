using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using ASA_Server_Manager.Interfaces.Wrappers;

namespace ASA_Server_Manager.Wrappers;

public class ProcessWrapper : IProcess
{
    #region Private Fields

    private Process _process;

    #endregion

    #region Implicit Operators

    public static implicit operator Process(ProcessWrapper wrapper) => wrapper._process;

    public static implicit operator ProcessWrapper(Process process) => new ProcessWrapper(process);

    #endregion

    #region Public Events

    public event EventHandler Disposed
    {
        add => _process.Disposed += value;
        remove => _process.Disposed -= value;
    }

    public event DataReceivedEventHandler ErrorDataReceived
    {
        add => _process.ErrorDataReceived += value;
        remove => _process.ErrorDataReceived -= value;
    }

    public event EventHandler Exited
    {
        add => _process.Exited += value;
        remove => _process.Exited -= value;
    }

    public event DataReceivedEventHandler OutputDataReceived
    {
        add => _process.OutputDataReceived += value;
        remove => _process.OutputDataReceived -= value;
    }

    #endregion

    #region Public Constructors

    public ProcessWrapper()
    {
        _process = new Process();
    }

    public ProcessWrapper(string fileName, string arguments = null)
    {
        _process = new Process();
        _process.StartInfo.FileName = fileName;
        _process.StartInfo.Arguments = arguments;
    }

    public ProcessWrapper(ProcessStartInfo startInfo)
    {
        _process = new Process();
        _process.StartInfo = startInfo;
    }

    public ProcessWrapper(Process process)
    {
        _process = process ?? new Process();
    }

    #endregion

    #region Public Properties

    public int BasePriority => _process.BasePriority;

    public Process BaseProcess => _process;

    public IContainer Container => _process.Container;

    public bool EnableRaisingEvents
    {
        get => _process.EnableRaisingEvents;
        set => _process.EnableRaisingEvents = value;
    }

    public int ExitCode => _process.ExitCode;

    public DateTime ExitTime => _process.ExitTime;

    public IntPtr Handle => _process.Handle;

    public int HandleCount => _process.HandleCount;

    public bool HasExited => _process.HasExited;

    public int Id => _process.Id;

    public string MachineName => _process.MachineName;

    public ProcessModule MainModule => _process.MainModule;

    public IntPtr MainWindowHandle => _process.MainWindowHandle;

    public string MainWindowTitle => _process.MainWindowTitle;

    public IntPtr MaxWorkingSet
    {
        get => _process.MaxWorkingSet;
        set => _process.MaxWorkingSet = value;
    }

    public IntPtr MinWorkingSet
    {
        get => _process.MinWorkingSet;
        set => _process.MinWorkingSet = value;
    }

    public ProcessModuleCollection Modules => _process.Modules;

    [Obsolete("This property has been deprecated.  Please use System.Diagnostics.Process.NonpagedSystemMemorySize64 instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
    public int NonpagedSystemMemorySize => _process.NonpagedSystemMemorySize;

    public long NonpagedSystemMemorySize64 => _process.NonpagedSystemMemorySize64;

    [Obsolete("This property has been deprecated.  Please use System.Diagnostics.Process.PagedMemorySize64 instead.  http://go.microsoft.com/")]
    public int PagedMemorySize => _process.PagedMemorySize;

    public long PagedMemorySize64 => _process.PagedMemorySize64;

    [Obsolete("This property has been deprecated.  Please use System.Diagnostics.Process.PagedSystemMemorySize64 instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
    public int PagedSystemMemorySize => _process.PagedSystemMemorySize;

    public long PagedSystemMemorySize64 => _process.PagedSystemMemorySize64;

    [Obsolete("This property has been deprecated.  Please use System.Diagnostics.Process.PeakPagedMemorySize64 instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
    public int PeakPagedMemorySize => _process.PeakPagedMemorySize;

    public long PeakPagedMemorySize64 => _process.PeakPagedMemorySize64;

    [Obsolete("This property has been deprecated.  Please use System.Diagnostics.Process.PeakVirtualMemorySize64 instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
    public int PeakVirtualMemorySize => _process.PeakVirtualMemorySize;

    public long PeakVirtualMemorySize64 => _process.PeakVirtualMemorySize64;

    [Obsolete("This property has been deprecated.  Please use System.Diagnostics.Process.PeakWorkingSet64 instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
    public int PeakWorkingSet => _process.PeakWorkingSet;

    public long PeakWorkingSet64 => _process.PeakWorkingSet64;

    public bool PriorityBoostEnabled
    {
        get => _process.PriorityBoostEnabled;
        set => _process.PriorityBoostEnabled = value;
    }

    public ProcessPriorityClass PriorityClass
    {
        get => _process.PriorityClass;
        set => _process.PriorityClass = value;
    }

    [Obsolete("This property has been deprecated.  Please use System.Diagnostics.Process.PrivateMemorySize64 instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
    public int PrivateMemorySize => _process.PrivateMemorySize;

    public long PrivateMemorySize64 => _process.PrivateMemorySize64;

    public TimeSpan PrivilegedProcessorTime => _process.PrivilegedProcessorTime;

    public string ProcessName => _process.ProcessName;

    public IntPtr ProcessorAffinity
    {
        get => _process.ProcessorAffinity;
        set => _process.ProcessorAffinity = value;
    }

    public bool Responding => _process.Responding;

    public int SessionId => _process.SessionId;

    public ISite Site
    {
        get => _process.Site;
        set => _process.Site = value;
    }

    public StreamReader StandardError => _process.StandardError;

    public StreamWriter StandardInput => _process.StandardInput;

    public StreamReader StandardOutput => _process.StandardOutput;

    public ProcessStartInfo StartInfo
    {
        get => _process.StartInfo;
        set => _process.StartInfo = value;
    }

    public DateTime StartTime => _process.StartTime;

    public ISynchronizeInvoke SynchronizingObject
    {
        get => _process.SynchronizingObject;
        set => _process.SynchronizingObject = value;
    }

    public ProcessThreadCollection Threads => _process.Threads;

    public TimeSpan TotalProcessorTime => _process.TotalProcessorTime;

    public TimeSpan UserProcessorTime => _process.UserProcessorTime;

    [Obsolete("This property has been deprecated.  Please use System.Diagnostics.Process.VirtualMemorySize64 instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
    public int VirtualMemorySize => _process.VirtualMemorySize;

    public long VirtualMemorySize64 => _process.VirtualMemorySize64;

    [Obsolete("This property has been deprecated.  Please use System.Diagnostics.Process.WorkingSet64 instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
    public int WorkingSet => _process.WorkingSet;

    public long WorkingSet64 => _process.WorkingSet64;

    #endregion

    #region Public Methods

    public void BeginErrorReadLine() => _process.BeginErrorReadLine();

    public void BeginOutputReadLine() => _process.BeginOutputReadLine();

    public void CancelErrorRead() => _process.CancelErrorRead();

    public void CancelOutputRead() => _process.CancelOutputRead();

    public void Close() => _process.Close();

    public bool CloseMainWindow() => _process.CloseMainWindow();

    public void Dispose()
    {
        _process?.Dispose();
        _process = null;
    }

    public void Kill() => _process.Kill();

    public void Refresh() => _process.Refresh();

    public bool Start() => _process.Start();

    public bool WaitForExit(int milliseconds) => _process.WaitForExit(milliseconds);

    public void WaitForExit() => _process.WaitForExit();

    public bool WaitForInputIdle(int milliseconds) => _process.WaitForInputIdle(milliseconds);

    public bool WaitForInputIdle() => _process.WaitForInputIdle();

    #endregion
}