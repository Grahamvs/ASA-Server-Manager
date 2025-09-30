using System.Text.RegularExpressions;
using System.Windows;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Configs;
using ASA_Server_Manager.Interfaces.Helpers;
using ASA_Server_Manager.Interfaces.Services;
using Octokit;
using Application = System.Windows.Application;

namespace ASA_Server_Manager.Services;

public class UpdateService : IUpdateService
{
    #region Private Fields

    private const string OwnerID = "Grahamvs";
    private const string RepoID = "ASA-Server-Manager";

    private readonly IAppSettingsService _appSettingsService;
    private readonly Version _currentVersion;
    private readonly IDialogService _dialogService;
    private readonly GitHubClient _gitHubClient;
    private readonly IProcessHelper _processHelper;
    private readonly Func<Window, IToastService> _toastServiceFunc;
    private readonly Regex _versionRegex = new(@"(\d+\.?){1,4}");
    private CancellationTokenSource _cancellationTokenSource;

    #endregion

    #region Public Constructors

    public UpdateService(
        IApplicationService applicationService,
        IAppSettingsService appSettingsService,
        IProcessHelper processHelper,
        IDialogService dialogService,
        Func<Window, IToastService> toastServiceFunc
    )
    {
        _appSettingsService = appSettingsService;
        _processHelper = processHelper;
        _dialogService = dialogService;
        _toastServiceFunc = toastServiceFunc;

        _gitHubClient = new GitHubClient(new ProductHeaderValue(OwnerID));
        _currentVersion = Version.Parse(applicationService.VersionString);

        _appSettingsService
            .FromPropertyChangedPattern()
            .WherePropertyIs(nameof(IAppSettings.CheckForAppUpdates))
            .Subscribe(
                _ =>
                {
                    Stop();
                    Start();
                }
            );
    }

    #endregion

    #region Public Methods

    public async Task CheckForUpdates(bool showNoUpdate, bool overrideIgnore, IToastService toastService = null)
    {
        var latest = (await GetValidReleases())
            .OrderByDescending(x => x.version)
            .FirstOrDefault();

        toastService ??= _toastServiceFunc(Application.Current?.MainWindow);

        if (latest.version is not { } latestVersion)
        {
            toastService.ShowError("Error checking for updates");
            return;
        }

        var newerThenCurrent = latestVersion.CompareTo(_currentVersion) > 0;

        var ignoredVersion = GetVersion(_appSettingsService.IgnoredAppVersion);
        var newerThenIgnored = latestVersion.CompareTo(ignoredVersion) > 0;

        var latestRelease = latest.release;

        if (newerThenCurrent && (overrideIgnore || newerThenIgnored))
        {
            if (newerThenIgnored && ignoredVersion is not null)
            {
                _appSettingsService.IgnoredAppVersion = null;
            }

            var versionText = latestRelease.Prerelease
                ? $"{latestVersion} (Pre-release)"
                : $"{latestVersion}";

            toastService.ShowInformation($"Version {versionText} is available.", LaunchLatestRelease, IgnoreLatestRelease);
        }
        else if (showNoUpdate)
        {
            toastService.ShowInformation("No new version available");
        }

        _appSettingsService.LastCheckedForAppUpdate = DateTime.Now;
        _appSettingsService.SaveSettings();

        return;

        //// Local Functions \\\\

        void LaunchLatestRelease() => _processHelper.RunWithShellExecute(latestRelease.HtmlUrl);

        void IgnoreLatestRelease()
        {
            var latestVersionString = latestVersion.ToString();
            var result = _dialogService.ShowMessage($"Do you wish to ignore notifications for update v{latestVersionString}?", buttons: MessageBoxButton.YesNo, icon: MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _appSettingsService.IgnoredAppVersion = latestVersionString;
                _appSettingsService.SaveSettings();
            }
        }
    }

    public void Start()
    {
        _cancellationTokenSource = new();

        Task.Run(
            async () =>
            {
                var token = _cancellationTokenSource.Token;
                var lastRun = _appSettingsService.LastCheckedForAppUpdate;

                while (!token.IsCancellationRequested)
                {
                    TimeSpan? delay = null;

                    if (lastRun.HasValue)
                    {
                        DateTime nextRun;

                        switch (_appSettingsService.CheckForAppUpdates)
                        {
                            // StartOnly is a special case, as we only check it when the app starts
                            case UpdateFrequency.Never:
                            case UpdateFrequency.OnStart:
                                await _cancellationTokenSource.CancelAsync();
                                continue;

                            case UpdateFrequency.Hourly:
                                nextRun = lastRun.Value + TimeSpan.FromHours(1);
                                break;

                            case UpdateFrequency.Daily:
                                nextRun = lastRun.Value + TimeSpan.FromDays(1);
                                break;

                            case UpdateFrequency.Weekly:
                                nextRun = lastRun.Value + TimeSpan.FromDays(7);
                                break;

                            case UpdateFrequency.Monthly:
                                nextRun = lastRun.Value + TimeSpan.FromDays(30);
                                break;

                            default: throw new ArgumentOutOfRangeException();
                        }

                        delay = nextRun - lastRun.Value;
                    }

                    if (delay.HasValue)
                    {
                        await Task.Delay(delay.Value, token);
                    }

                    await CheckForUpdates(false, false);
                }
            }
        );
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }

    #endregion

    #region Private Methods

    private async Task<IReadOnlyList<(Release release, Version version)>> GetValidReleases()
    {
        var includePreReleases = _appSettingsService.IncludePreReleases;

        var releases = await _gitHubClient.Repository.Release.GetAll(OwnerID, RepoID);

        var validReleases = releases
            .Where(r => !r.Draft && (includePreReleases || !r.Prerelease))
            .Select(r => (release: r, version: GetVersion(r.TagName)))
            .Where(x => x.version != null)
            .ToList()
            .AsReadOnly();

        return validReleases;
    }

    private Version GetVersion(string version)
    {
        var matches = _versionRegex.Matches(version ?? string.Empty);

        if (matches.Count <= 0)
            return null;

        var cleanedVersion = string.Join('.', matches[0].Value.Split('.').Take(4));

        return Version.TryParse(cleanedVersion, out var result)
            ? result
            : null;
    }

    #endregion
}