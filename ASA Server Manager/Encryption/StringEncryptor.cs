using System.Text;

namespace ASA_Server_Manager.Encryption;

public static class StringEncryptor
{
    private const string EncryptKey = "1MystWalkers2Angels3Phoenix";

    public static string Encrypt(string value, string key = null)
    {
        int Transformer(int val1, int val2) => val1 + val2 > 255 ? val1 + val2 - 255 : val1 + val2;

        return Transform(value, Transformer, key);
    }

    public static string Decrypt(string value, string key = null)
    {
        int Transformer(int val1, int val2) => val1 - val2 < 0 ? val1 - val2 + 255 : val1 - val2;

        return Transform(value, Transformer, key);
    }

    private static string Transform(string value, Func<int, int, int> transformation, string key)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(key))
        {
            key = EncryptKey;
        }

        var values = value.ToCharArray();
        var result = new StringBuilder();
        var encryptMap = CreateMap(values.Length, key);

        for (var x = 0; x < values.Length; x++)
        {
            var ascVal = transformation(values[x], encryptMap[x]);
            result.Append((char) ascVal);
        }

        return result.ToString();
    }

    private static StringBuilder CreateMap(int length, string key)
    {
        var result = new StringBuilder();

        for (long x = 1; x <= length; x += key.Length)
        {
            result.Append(key);
        }

        return result.Append(result.ToString()[..length]);
    }
}