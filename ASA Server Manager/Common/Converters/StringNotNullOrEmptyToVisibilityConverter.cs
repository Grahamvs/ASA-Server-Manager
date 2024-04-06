using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ASA_Server_Manager.Common.Converters;

// Create a string not null or empty to visibility converter
public class StringNotNullOrEmptyToVisibilityConverter : MarkupExtension, IValueConverter
{
    #region Public Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is string stringValue
            ? !string.IsNullOrEmpty(stringValue)
                ? Visibility.Visible
                : Visibility.Collapsed
            : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    #endregion
}