using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ASA_Server_Manager.Common.Converters;

public class NegatedBooleanToVisibilityConverter : MarkupExtension, IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) =>
        value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) =>
        value is Visibility.Collapsed;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}