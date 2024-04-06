using System.Windows.Input;
using ASA_Server_Manager.Extensions;

namespace ASA_Server_Manager.Common.Commands;

public class CompositeCommand : ICommand
{
    #region Private Fields

    private readonly List<ICommand> _commands;

    #endregion

    #region Public Events

    public event EventHandler CanExecuteChanged;

    #endregion

    #region Public Constructors

    public CompositeCommand(params ICommand[] commands)
        : this((IEnumerable<ICommand>) commands)
    {
    }

    public CompositeCommand(IEnumerable<ICommand> commands)
        : this()
    {
        commands.ToList().ForEach(AddCommand);
    }

    public CompositeCommand()
    {
        _commands = [];
    }

    #endregion

    #region Public Properties

    public Action<object> BeginExecuteAction { get; set; }

    public Action<object> EndExecuteAction { get; set; }

    public Action<Exception> ErrorHandler { get; set; }

    #endregion

    #region Public Methods

    public void AddCommand(ICommand command)
    {
        _commands.Add(command ?? throw new ArgumentNullException(nameof(command)));

        command.CanExecuteChanged += Command_CanExecuteChanged;
    }

    public bool CanExecute(object parameter) => _commands.All(command => command.CanExecute(parameter));

    public void Clear() => _commands.Clear(command => command.CanExecuteChanged -= Command_CanExecuteChanged);

    public void Execute(object parameter)
    {
        try
        {
            BeginExecuteAction?.Invoke(parameter);

            foreach (var command in _commands)
            {
                command.Execute(parameter);
            }
        }
        catch (Exception exception)
        {
            if (ErrorHandler == null)
            {
                throw;
            }

            ErrorHandler(exception);
        }
        finally
        {
            EndExecuteAction?.Invoke(parameter);
        }
    }

    public bool Remove(ICommand command) => _commands.Remove(command ?? throw new ArgumentNullException(nameof(command)), cmd => cmd.CanExecuteChanged -= Command_CanExecuteChanged);

    #endregion

    #region Private Methods

    private void Command_CanExecuteChanged(object sender, EventArgs e) => CanExecuteChanged?.Invoke(this, e);

    #endregion
}