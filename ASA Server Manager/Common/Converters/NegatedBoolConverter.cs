using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ASA_Server_Manager.Common.Converters;

public class NegatedBoolConverter : MarkupExtension, IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) =>
        value is bool val ? !val : value;

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) =>
        Convert(
            value,
            targetType,
            parameter,
            culture
        );

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}