using ASA_Server_Manager.Attributes;

namespace ASA_Server_Manager_Tests.Serialization.HelperClasses;

public class SerializeTestClassWithIgnore
{
    public string Property1 { get; set; }

    public string Property2 { get; set; }

    [DoNotSerialize]
    public string SecretProperty { get; set; }
}