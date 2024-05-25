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
    private readonly IModService _modService;
    private ObservableCollection<Mod> _availableModsList;
    private string _filterText;
    private bool _isBusy;
    private ICollectionView _modListView;
    private Mod _selectedMod;

    #endregion

    #region Public Constructors

    public AvailableModsViewModel(
        IApplicationService applicationService,
        IModService modService,
        IDialogService dialogService)
    {
        _applicationService = applicationService;
        _modService = modService;

        var baseCommand = new ActionCommand(() => { }, () => !IsBusy)
            .ObservesProperty(() => IsBusy);

        SaveCommand = CreateBasedCommand(new ActionCommand(ExecuteSaveCommand));

        var selectedItemBaseCommand = new ActionCommand(() => { }, () => SelectedMod != null)
            .ObservesProperty(() => SelectedMod);

        DeleteCommand = CreateBasedCommand(
            selectedItemBaseCommand,
            new ActionCommand(() => _availableModsList.Remove(SelectedMod))
        );

        MoveSelectedItemCommand = CreateBasedCommand(
            selectedItemBaseCommand,
            new ActionCommand<MoveDirection>(ExecuteMoveSelectedModCommand, CanExecuteMoveSelectedModCommand)
        );

        //// Local Functions \\\\
        return;

        CompositeCommand CreateBasedCommand(params ICommand[] commands)
        {
            var commandList = new List<ICommand> { baseCommand };
            commandList.AddRange(commands);

            return new(commandList.ToArray())
            {
                ErrorHandler = exception => dialogService.ShowErrorMessage(exception.Message),
                BeginExecuteAction = _ => IsBusy = true,
                EndExecuteAction = _ => IsBusy = false,
            };
        }
    }

    #endregion

    #region Public Properties

    public ICommand DeleteCommand { get; }

    public string FilterText
    {
        get => _filterText;
        set => SetProperty(ref _filterText, value, ModListView.Refresh);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public ICollectionView ModListView => _modListView;

    public ICommand MoveSelectedItemCommand { get; }

    public ICommand SaveCommand { get; }

    public Mod SelectedMod
    {
        get => _selectedMod;
        set => SetProperty(ref _selectedMod, value);
    }

    public string WindowTitle => $"{_applicationService.ExeName}: Available Mods";

    #endregion

    #region Protected Methods

    protected override void OnLoad()
    {
        _availableModsList = new ObservableCollection<Mod>(_modService.AvailableModsList);

        var viewSource = CollectionViewSource.GetDefaultView(_availableModsList);
        viewSource.Filter = OnFilter;
        _modListView = viewSource;

        RaisePropertyChanged(nameof(ModListView));
    }

    protected override void OnUnload()
    {
    }

    #endregion

    #region Private Methods

    private bool CanExecuteMoveSelectedModCommand(MoveDirection direction)
    {
        var index = _availableModsList.IndexOf(SelectedMod);

        var canMove = direction switch
        {
            MoveDirection.Top => index > 0,
            MoveDirection.Up => index > 0,
            MoveDirection.Down => index < _availableModsList.Count - 1,
            MoveDirection.Bottom => index < _availableModsList.Count - 1,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        return canMove;
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
            MoveDirection.Top => 0,
            MoveDirection.Up => oldIndex - 1,
            MoveDirection.Down => oldIndex + 1,
            MoveDirection.Bottom => _availableModsList.Count - 1,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        _availableModsList.Move(oldIndex, newIndex);

        RaisePropertyChanged(nameof(SelectedMod));
    }

    private void ExecuteSaveCommand()
    {
        _modService.SetMods(_availableModsList);

        _modService.Save();

        RaiseCloseRequested(true);
    }

    private bool OnFilter(object obj)
    {
        if (obj is not Mod mod)
            return false;

        if (FilterText.IsNullOrEmpty())
            return true;

        return CheckValue(mod.ID.ToString())
            || CheckValue(mod.Name)
            || CheckValue(mod.Comments);

        bool CheckValue(string value) => (value ?? string.Empty).Contains(FilterText, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}