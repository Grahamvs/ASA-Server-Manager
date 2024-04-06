using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ASA_Server_Manager.Common.Converters
{
    public class IntToDoubleConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is int intValue
                ? System.Convert.ToDouble(intValue)
                : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is double doubleValue
                ? System.Convert.ToInt32(doubleValue)
                : null;

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}