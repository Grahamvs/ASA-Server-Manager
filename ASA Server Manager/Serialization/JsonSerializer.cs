using System.Reflection;
using ASA_Server_Manager.Attributes;
using ASA_Server_Manager.Interfaces.Serialization;
using ASA_Server_Manager.Interfaces.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ASA_Server_Manager.Serialization;

public class JsonSerializer : ISerializer
{
    private readonly IFileSystemService _fileSystemService;

    #region Private Fields

    private readonly JsonSerializerSettings _settings = new()
    {
        ContractResolver = new CustomContractResolver(),
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter> { new StringEnumConverter() },
    };

    public JsonSerializer(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
    }

    #endregion

    #region Public Methods

    public void SerializeToFile<T>(T value, string filePath)
    {
        var result = Serialize(value);

        _fileSystemService.WriteAllText(filePath, result);
    }

    public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value, _settings);

    public T DeserializeFromFile<T>(string filePath)
    {
        var value = _fileSystemService.ReadAllText(filePath);

        return Deserialize<T>(value);
    }

    public string Serialize<T>(T value) => JsonConvert.SerializeObject(value, _settings);

    #endregion

    #region Private Classes

    private class CustomContractResolver : DefaultContractResolver
    {
        #region Protected Methods

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            // Existing code
            property.ShouldSerialize = instance =>
            {
                var propValue = member.MemberType == MemberTypes.Property
                    ? ((PropertyInfo)member).GetValue(instance)
                    : ((FieldInfo)member).GetValue(instance);

                return propValue != null && member.GetCustomAttribute<DoNotSerializeAttribute>() is null;
            };

            return property;
        }

        #endregion
    }

    #endregion
}