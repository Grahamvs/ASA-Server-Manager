using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Markup;
using ASA_Server_Manager.Attributes;

namespace ASA_Server_Manager.Common.Converters
{
    public class EnumToDisplayValueConverter : MarkupExtension, IValueConverter
    {
        #region Public Methods

        public static string Convert(Enum value)
        {
            if (value == null)
                return null;

            var attribute = value
                .GetType()
                .GetField(value.ToString())
                ?.GetCustomAttributes(typeof(EnumDisplayValueAttribute), false)
                .FirstOrDefault() as EnumDisplayValueAttribute;

            // If there is no display value set, attempt to split the text on camel casing.
            return attribute != null
                ? attribute.DisplayValue
                : Regex.Replace(value.ToString(), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Convert(value as Enum);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        #endregion
    }
}