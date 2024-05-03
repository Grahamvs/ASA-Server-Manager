using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using ASA_Server_Manager.Common.Commands;
using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Extensions;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Interfaces.ViewModels;

namespace ASA_Server_Manager.ViewModels;

public class AvailableModsViewModel : WindowViewModel, IAvailableModsViewModel
{
    #region Private Fields

    private readonly IApplicationService _applicationService;
    private readonly ActionCommand _deleteModCommand;
    private readonly IModService _modService;
    private readonly ActionCommand<MoveDirection> _moveSelectedModCommand;
    private readonly ActionCommand _saveCommand;
    private ObservableCollection<Mod> _availableModsList;
    private bool _isBusy;
    private string _modFilterText;
    private ICollectionView _modListView;
    private Mod _selectedMod;

    #endregion

    #region Public Constructors

    public AvailableModsViewModel(
        IApplicationService applicationService,
        IModService modService
    )
    {
        _applicationService = applicationService;
        _modService = modService;

        _saveCommand = new ActionCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
        _deleteModCommand = new ActionCommand(ExecuteDeleteModCommand, CanExecuteDeleteModCommand);
        _moveSelectedModCommand = new ActionCommand<MoveDirection>(ExecuteMoveSelectedModCommand, CanExecuteMoveSelectedModCommand);
    }

    #endregion

    #region Public Properties

    public ICommand DeleteModCommand => _deleteModCommand;

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value, OnIsBusyChanged);
    }

    public string ModFilterText
    {
        get => _modFilterText;
        set => SetProperty(ref _modFilterText, value, ModListView.Refresh);
    }

    public ICollectionView ModListView => _modListView;

    public ICommand MoveSelectedModCommand => _moveSelectedModCommand;

    public ICommand SaveCommand => _saveCommand;

    public Mod SelectedMod
    {
        get => _selectedMod;
        set => SetProperty(ref _selectedMod, value, OnSelectedModChanged);
    }

    public string WindowTitle => $"{_applicationService.ExeName}: Available Mods";

    #endregion

    #region Protected Methods

    protected override void OnLoad()
    {
        _availableModsList = new ObservableCollection<Mod>(_modService.AvailableModsList);

        var viewSource = CollectionViewSource.GetDefaultView(_availableModsList);
        viewSource.Filter = OnModsFilter;
        _modListView = viewSource;

        RaisePropertyChanged(nameof(ModListView));
    }

    protected override void OnUnload()
    {
    }

    #endregion

    #region Private Methods

    private bool CanExecuteDeleteModCommand() => !IsBusy && SelectedMod != null;

    private bool CanExecuteMoveSelectedModCommand(MoveDirection direction)
    {
        if (IsBusy || SelectedMod == null)
        {
            return false;
        }

        var index = _availableModsList.IndexOf(SelectedMod);

        var canMove = direction switch
        {
            MoveDirection.Up => index > 0,
            MoveDirection.Down => index < _availableModsList.Count - 1,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        return canMove;
    }

    private bool CanExecuteSaveCommand() => !IsBusy;

    private void ExecuteDeleteModCommand()
    {
        _availableModsList.Remove(SelectedMod);
    }

    private void ExecuteMoveSelectedModCommand(MoveDirection direction)
    {
        if (!CanExecuteMoveSelectedModCommand(direction))
        {
            return;
        }

        var oldIndex = _availableModsList.IndexOf(SelectedMod);

        var newIndex = direction switch
        {
            MoveDirection.Up => oldIndex - 1,
            MoveDirection.Down => oldIndex + 1,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        _availableModsList.Move(oldIndex, newIndex);

        _moveSelectedModCommand.RaiseCanExecuteChanged();
    }

    private void ExecuteSaveCommand()
    {
        if (!CanExecuteSaveCommand())
        {
            return;
        }

        try
        {
            IsBusy = true;

            _modService.SetMods(_availableModsList);

            _modService.Save();

            RaiseCloseRequested(true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnIsBusyChanged()
    {
        UpdateButtonStates();
    }

    private bool OnModsFilter(object obj)
    {
        if (obj is not Mod mod)
            return false;

        if (ModFilterText.IsNullOrEmpty())
            return true;

        return CheckValue(mod.ID.ToString())
            || CheckValue(mod.Name)
            || CheckValue(mod.Comments);

        bool CheckValue(string value) => (value ?? string.Empty).Contains(ModFilterText, StringComparison.OrdinalIgnoreCase);
    }

    private void OnSelectedModChanged()
    {
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        _moveSelectedModCommand.RaiseCanExecuteChanged();
        _deleteModCommand.RaiseCanExecuteChanged();
        _saveCommand.RaiseCanExecuteChanged();
    }

    #endregion
}