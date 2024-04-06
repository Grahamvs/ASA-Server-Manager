using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace ASA_Server_Manager.Extensions;

public static class ObservableExtensions
{
    public static IObservable<EventPattern<PropertyChangedEventArgs>> WherePropertiesAre(this IObservable<EventPattern<PropertyChangedEventArgs>> source, params string[] validProperties) => WherePropertiesAre(source, (IEnumerable<string>) validProperties);

    public static IObservable<EventPattern<PropertyChangedEventArgs>> WherePropertiesAre(this IObservable<EventPattern<PropertyChangedEventArgs>> source, IEnumerable<string> validProperties)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (validProperties == null)
            throw new ArgumentNullException(nameof(validProperties));

        return source.Where(pattern => validProperties.Contains(pattern.EventArgs.PropertyName, StringComparer.OrdinalIgnoreCase));
    }

    public static IObservable<EventPattern<PropertyChangedEventArgs>> WherePropertyIs(this IObservable<EventPattern<PropertyChangedEventArgs>> source, string validProperty)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (validProperty.IsNullOrWhiteSpace())
            throw new ArgumentException(nameof(validProperty));

        return source.Where(pattern => string.Equals(pattern.EventArgs.PropertyName, validProperty, StringComparison.OrdinalIgnoreCase));
    }
}