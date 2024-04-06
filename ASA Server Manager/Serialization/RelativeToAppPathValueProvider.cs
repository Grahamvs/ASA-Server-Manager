using System.IO;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace ASA_Server_Manager.Serialization;

public class RelativeToAppPathValueProvider : IValueProvider
{
    private static readonly string AppPath = Directory.GetCurrentDirectory();
    private readonly PropertyInfo _propertyInfo;

    public RelativeToAppPathValueProvider(PropertyInfo propertyInfo)
    {
        _propertyInfo = propertyInfo;
    }

    public object GetValue(object target)
    {
        var value = _propertyInfo.GetValue(target);

        return value is string absolutePath
            ? Path.GetRelativePath(AppPath, absolutePath)
            : value;
    }

    public void SetValue(object target, object value) => _propertyInfo.SetValue(target, value is string relativePath ? ConvertToAbsolutePath(relativePath) : value);

    private string ConvertToAbsolutePath(string relativePath) => Path.GetFullPath(relativePath, AppPath);
}