using System.IO;
using System.Windows;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Services;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ASA_Server_Manager.Services;

public class DialogService : IDialogService
{
    #region Private Fields

    private readonly IApplicationService _applicationService;

    #endregion

    #region Public Constructors

    public DialogService(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    #endregion

    #region Public Methods

    public string OpenFileDialog(string filter, string initialDirectory = null, string fileName = null, string title = null)
    {
        var dialog = new CommonOpenFileDialog
        {
            IsFolderPicker = false,
            InitialDirectory = initialDirectory ?? _applicationService.WorkingDirectory,
            DefaultFileName = fileName,
            EnsureFileExists = true,
            Multiselect = false,
            Title = title,
        };

        SplitFilterString(filter ?? "Any file|*")?.ForEach(dialog.Filters.Add);

        return dialog.ShowDialog() == CommonFileDialogResult.Ok
            ? dialog.FileName
            : null;
    }

    public string OpenFolderDialog(string initialDirectory = null, string title = null)
    {
        var dialog = new CommonOpenFileDialog
        {
            IsFolderPicker = true,
            InitialDirectory = initialDirectory,
            Multiselect = false,
            Title = title
        };

        return dialog.ShowDialog() == CommonFileDialogResult.Ok
            ? dialog.FileName
            : null;
    }

    public string SaveFileDialog(string defaultExt, string filter, string fileName = null, string title = null)
    {
        fileName ??= string.Empty;

        // Create SaveFileDialog
        var dialog = new CommonSaveFileDialog
        {
            DefaultExtension = $".{defaultExt.Trim(' ', '.')}",
            DefaultFileName = Path.GetFileName(fileName),
            InitialDirectory = Path.GetDirectoryName(fileName),
            OverwritePrompt = true
        };

        SplitFilterString(filter ?? "Any file|*")?.ForEach(dialog.Filters.Add);

        // Get the selected file name and display in a TextBox
        return dialog.ShowDialog() == CommonFileDialogResult.Ok
            ? dialog.FileName
            : null;
    }

    public MessageBoxResult ShowErrorMessage(
        string message,
        string caption = null,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxResult defaultResponse = MessageBoxResult.OK
    ) =>
        ShowMessage(message, $"{_applicationService.ExeName}: {caption ?? "Error"}", buttons, MessageBoxImage.Error, defaultResponse);

    public MessageBoxResult ShowMessage(
        string message,
        string caption = null,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None,
        MessageBoxResult defaultResponse = MessageBoxResult.OK
    ) =>
        MessageBox.Show(message, caption ?? _applicationService.ExeName, buttons, icon, defaultResponse);

    public MessageBoxResult ShowWarningMessage(
        string message,
        string caption = null,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxResult defaultResponse = MessageBoxResult.OK
    ) =>
        ShowMessage(message, $"{_applicationService.ExeName}: {caption ?? "Warning"}", buttons, MessageBoxImage.Warning, defaultResponse);

    #endregion

    #region Private Methods

    private IReadOnlyList<CommonFileDialogFilter> SplitFilterString(string filter)
    {
        if (filter.IsNullOrWhiteSpace())
        {
            return null;
        }

        var filterParts = filter.Split('|');

        var filters = new List<CommonFileDialogFilter>();

        for (var i = 0; i < filterParts.Length; i += 2)
        {
            var displayName = filterParts[i];

            var extension = i + 1 < filterParts.Length
                ? filterParts[i + 1]
                : string.Empty;

            if (!string.IsNullOrEmpty(extension))
            {
                filters.Add(new CommonFileDialogFilter(displayName, extension));
            }
        }

        return filters;
    }

    #endregion
}