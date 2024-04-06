using System.ComponentModel;
using System.Runtime.CompilerServices;
using ASA_Server_Manager.Extensions;

namespace ASA_Server_Manager.Common;

public abstract class BindableBase : INotifyPropertyChanged
{
    #region Public Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Protected Methods

    protected static void DisposeField<T>(ref T disposable)
        where T : IDisposable
    {
        disposable?.Dispose();
        disposable = default;
    }

    protected void RaisePropertiesChanged(params string[] propertyNames) =>
        propertyNames
            .Where(value => !value.IsNullOrWhiteSpace())
            .ToList()
            .ForEach(RaisePropertyChanged);

    protected virtual void OnRaisePropertyChanged(string propertyName)
    {
    }

    protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        OnRaisePropertyChanged(propertyName);
    }

    protected bool SetPrivateProperty<T>(ref T field, T value, params string[] propertiesToRaise) => SetPrivateProperty(ref field, value, null, propertiesToRaise);

    protected bool SetPrivateProperty<T>(ref T field, T value, Action onChanged, params string[] propertiesToRaise)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;

        foreach (var property in propertiesToRaise)
        {
            RaisePropertyChanged(property);
        }

        onChanged?.Invoke();

        return true;
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null) => SetProperty(ref field, value, null, propertyName);

    protected bool SetProperty<T>(ref T field, T value, Action onChanged, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        RaisePropertyChanged(propertyName);
        onChanged?.Invoke();

        return true;
    }

    #endregion
}