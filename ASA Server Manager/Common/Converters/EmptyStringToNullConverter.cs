using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ASA_Server_Manager.Common.Converters;

public class EmptyStringToNullConverter : MarkupExtension, IValueConverter
{
    #region Public Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value ?? string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => string.IsNullOrWhiteSpace(value as string) ? null : (string)value;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    #endregion
}