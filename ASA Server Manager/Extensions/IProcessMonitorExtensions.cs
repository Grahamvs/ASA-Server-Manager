using System.Reactive;
using System.Reactive.Linq;
using ASA_Server_Manager.Interfaces.Helpers;

namespace ASA_Server_Manager.Extensions;

public static class IProcessMonitorExtensions
{
    public static IObservable<EventPattern<bool>> FromIsRunningChangedPattern(this IProcessMonitor source) =>
        source == null
            ? throw new ArgumentNullException(nameof(source))
            : Observable.FromEventPattern<bool>(
                h => source.IsRunningChanged += h,
                h => source.IsRunningChanged -= h
            );
}