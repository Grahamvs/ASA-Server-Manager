using ASA_Server_Manager.Interfaces.Helpers;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Interfaces.ViewModels;

namespace ASA_Server_Manager.ViewModels;

public class AboutViewModel : WindowViewModel, IAboutViewModel
{
    #region Private Fields

    private readonly IApplicationService _applicationService;
    private readonly IProcessHelper _processHelper;

    #endregion

    #region Public Constructors

    public AboutViewModel(
        IApplicationService applicationService,
        IProcessHelper processHelper)
    {
        _applicationService = applicationService;
        _processHelper = processHelper;
    }

    #endregion

    #region Public Properties

    public string Title => _applicationService.Title;

    public string Version => _applicationService.VersionString;

    public string WindowTitle => $"{_applicationService.ExeName}: About";

    #endregion

    #region Protected Methods

    protected override void OnLoad()
    {
    }

    protected override void OnUnload()
    {
    }

    #endregion

    public void OpenWeblink(string url) => _processHelper.OpenWeblink(url);
}