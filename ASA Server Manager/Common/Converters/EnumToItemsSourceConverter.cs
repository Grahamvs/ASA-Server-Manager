using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ASA_Server_Manager.Common.Converters;

public class EnumToItemsSourceConverter : MarkupExtension, IValueConverter
{
    #region Public Methods

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is Type { IsEnum: true } type
            ? Enum.GetValues(type)
            : null;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    #endregion
}