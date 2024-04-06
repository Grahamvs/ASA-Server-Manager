using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace ASA_Server_Manager.Extensions;

public static class INotifyPropertyChangedExtensions
{
    public static IObservable<EventPattern<PropertyChangedEventArgs>> FromPropertyChangedPattern(this INotifyPropertyChanged source) =>
        source == null
            ? throw new ArgumentNullException(nameof(source))
            : Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h
            );
}