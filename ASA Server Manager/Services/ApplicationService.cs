using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using ASA_Server_Manager.Interfaces.Services;

namespace ASA_Server_Manager.Services;

public class ApplicationService : IApplicationService
{
    #region Public Constructors

    public ApplicationService()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();

        T GetAttribute<T>()
            where T : Attribute
        {
            return (T) executingAssembly.GetCustomAttribute(typeof(T));
        }

        var fileName = Process.GetCurrentProcess().MainModule?.FileName ?? throw new InvalidOperationException("Cannot determine name of executable.");
        var filePath = new FileInfo(fileName);
        ExePath = filePath.FullName;
        ExeName = Path.GetFileNameWithoutExtension(filePath.Name);
        ExeDirectory = filePath.Directory?.FullName;

        WorkingDirectory = Directory.GetCurrentDirectory();

        Title = GetAttribute<AssemblyTitleAttribute>()?.Title;
        Description = GetAttribute<AssemblyDescriptionAttribute>()?.Description;
        TradeMark = GetAttribute<AssemblyTrademarkAttribute>()?.Trademark;
        Company = GetAttribute<AssemblyCompanyAttribute>()?.Company;
        Copyright = GetAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        VersionString = executingAssembly.GetName().Version?.ToString();
    }

    #endregion

    #region Public Properties

    public string Company { get; }

    public string Copyright { get; }

    public string Description { get; }

    public string ExeDirectory { get; }

    public string ExeName { get; }

    public string ExePath { get; }

    public string Name { get; }

    public string Title { get; }

    public string TradeMark { get; }

    public string VersionString { get; }

    public string WorkingDirectory { get; }

    #endregion

    #region Public Methods

    public void Shutdown()
    {
        Application.Current.Shutdown();
    }

    #endregion
}