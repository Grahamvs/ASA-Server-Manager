using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ASA_Server_Manager.Common.Commands;

public sealed class CommandParameter : Freezable, IDisposable, INotifyPropertyChanged
{
    #region Public Fields

    public static readonly DependencyProperty InitialFeedBackProperty = DependencyProperty.Register(nameof(InitialFeedBack), typeof(string), typeof(CommandParameter), new PropertyMetadata(null));

    public static readonly DependencyProperty ParameterProperty = DependencyProperty.Register(nameof(Parameter), typeof(object), typeof(CommandParameter), new PropertyMetadata(default(object)));

    #endregion

    #region Private Fields

    private CancellationTokenSource _cancellationTokenSource = new();
    private string _feedback;

    #endregion

    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Public Properties

    public CancellationToken CancellationToken => _cancellationTokenSource.Token;

    public string Feedback
    {
        get => _feedback ?? InitialFeedBack;
        private set => SetField(ref _feedback, value);
    }

    public string InitialFeedBack
    {
        get => (string) GetValue(InitialFeedBackProperty);
        set => SetValue(InitialFeedBackProperty, value);
    }

    public bool IsCancelled
    {
        get => _cancellationTokenSource.IsCancellationRequested;
    }

    public object Parameter
    {
        get => GetValue(ParameterProperty);
        set => SetValue(ParameterProperty, value);
    }

    #endregion

    #region Public Methods

    public void Cancel() => _cancellationTokenSource.Cancel();

    public void Dispose()
    {
        ClearTokenSource();
    }

    public void Reset()
    {
        Feedback = null;

        ClearTokenSource();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public bool SetFeedback(bool condition, string invalidFeedback)
    {
        if (!condition)
        {
            Feedback = invalidFeedback;
        }

        return condition;
    }

    #endregion

    #region Protected Methods

    protected override Freezable CreateInstanceCore() => new CommandParameter();

    #endregion

    #region Private Methods

    private void ClearTokenSource()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, Action onChangedAction = null, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;

        onChangedAction?.Invoke();

        RaisePropertyChanged(propertyName);

        return true;
    }

    #endregion
}