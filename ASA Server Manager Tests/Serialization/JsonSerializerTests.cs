using ASA_Server_Manager.Serialization;

namespace ASA_Server_Manager_Tests.Serialization;

[TestClass]
public class JsonSerializerTests : BaseSerializationServiceTests<JsonSerializer>
{
    protected override string GetSerializeTestClassString(string property1, string property2, string property3)
        => $"{{\r\n  \"Property1\": \"{property1}\",\r\n  \"Property2\": \"{property2}\",\r\n  \"Property3\": \"{property3}\"\r\n}}";

    protected override string GetSerializeTestClassWithIgnoreString(string property1, string property2)
        => $"{{\r\n  \"Property1\": \"{property1}\",\r\n  \"Property2\": \"{property2}\"\r\n}}";
}