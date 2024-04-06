namespace ASA_Server_Manager.Interfaces.Serialization;

public interface ISerializer
{
    T Deserialize<T>(string value);

    T DeserializeFromFile<T>(string filePath);

    string Serialize<T>(T value);

    void SerializeToFile<T>(T value, string filePath);
}