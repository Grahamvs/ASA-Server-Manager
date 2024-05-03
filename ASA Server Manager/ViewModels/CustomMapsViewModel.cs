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

namespace ASA_Server_Manager.ViewModels
{
    public class CustomMapsViewModel : WindowViewModel, ICustomMapsViewModel
    {
        #region Private Fields

        private readonly IApplicationService _applicationService;
        private readonly IMapService _mapService;
        private string _filterText;
        private bool _isBusy;
        private ObservableCollection<MapDetails> _mapList;
        private ICollectionView _mapListView;
        private MapDetails _selectedMap;

        #endregion

        #region Public Constructors

        public CustomMapsViewModel(
            IApplicationService applicationService,
            IMapService mapService,
            IDialogService dialogService)
        {
            _applicationService = applicationService;
            _mapService = mapService;

            var baseCommand = new ActionCommand(() => { }, () => !IsBusy)
                .ObservesProperty(() => IsBusy);

            SaveCommand = CreateBasedCommand(new ActionCommand(ExecuteSaveCommand));

            var selectedItemBaseCommand = new ActionCommand(() => { }, () => SelectedMap != null)
                .ObservesProperty(() => SelectedMap);

            DeleteCommand = CreateBasedCommand(
                selectedItemBaseCommand,
                new ActionCommand(() => _mapList.Remove(SelectedMap))
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
            set => SetProperty(ref _filterText, value, MapListView.Refresh);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public ICollectionView MapListView => _mapListView;

        public ICommand MoveSelectedItemCommand { get; }

        public ICommand SaveCommand { get; }

        public MapDetails SelectedMap
        {
            get => _selectedMap;
            set => SetProperty(ref _selectedMap, value);
        }

        public string WindowTitle => $"{_applicationService.ExeName}: Custom Maps";

        #endregion

        #region Protected Methods

        protected override void OnLoad()
        {
            _mapList = new ObservableCollection<MapDetails>(_mapService.CustomMaps);

            var viewSource = CollectionViewSource.GetDefaultView(_mapList);
            viewSource.Filter = OnFilter;
            _mapListView = viewSource;

            RaisePropertyChanged(nameof(MapListView));
        }

        protected override void OnUnload()
        {
        }

        #endregion

        #region Private Methods

        private bool CanExecuteMoveSelectedModCommand(MoveDirection direction)
        {
            var index = _mapList.IndexOf(SelectedMap);

            var canMove = direction switch
            {
                MoveDirection.Up => index > 0,
                MoveDirection.Down => index < _mapList.Count - 1,
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

            var oldIndex = _mapList.IndexOf(SelectedMap);

            var newIndex = direction switch
            {
                MoveDirection.Up => oldIndex - 1,
                MoveDirection.Down => oldIndex + 1,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

            _mapList.Move(oldIndex, newIndex);

            RaisePropertyChanged(nameof(SelectedMap));
        }

        private void ExecuteSaveCommand()
        {
            _mapService.SetCustomMaps(_mapList);

            _mapService.Save();

            RaiseCloseRequested(true);
        }

        private bool OnFilter(object obj)
        {
            if (obj is not MapDetails mapDetails)
                return false;

            if (FilterText.IsNullOrEmpty())
                return true;

            return CheckValue(mapDetails.ID)
                || CheckValue(mapDetails.Name);

            bool CheckValue(string value) => (value ?? string.Empty).Contains(FilterText, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}