using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Interfaces.ViewModels;

namespace ASA_Server_Manager.ViewModels;

public class AboutViewModel : WindowViewModel, IAboutViewModel
{
    #region Private Fields

    private readonly IApplicationService _applicationService;

    #endregion

    #region Public Constructors

    public AboutViewModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
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
}