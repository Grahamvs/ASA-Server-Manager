using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ASA_Server_Manager.Common.Converters;

public class NotNullConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}