namespace ASA_Server_Manager.Attributes
{
    public class EnumDisplayValueAttribute : Attribute
    {
        public string DisplayValue { get; }

        public EnumDisplayValueAttribute(string displayValue)
        {
            DisplayValue = displayValue;
        }
    }
}