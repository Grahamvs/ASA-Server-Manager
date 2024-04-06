using ASA_Server_Manager.Encryption;
using FluentAssertions;

namespace ASA_Server_Manager_Tests.Encryption;

[TestClass]
public class StringEncryptorTests
{
    #region Private Fields

    private static readonly string AllCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890`~!@#$%^&*()-_=+[{]}\\|;:'\",<.>/?";

    #endregion

    #region Setup

    public static IEnumerable<object[]> GetTestKeys()
    {
        yield return [""];
        yield return ["key"];
        yield return ["Password"];
        yield return ["Password123"];
        yield return ["This is a test key"];
        yield return ["TestEncryptionKey"];
    }

    #endregion

    #region Tests

    [DataTestMethod]
    [DynamicData(nameof(GetTestKeys), DynamicDataSourceType.Method)]
    public void Can_Encrypt_And_Decrypt_Value_With_Key(string key)
    {
        // Arrange

        var encrypted = StringEncryptor.Encrypt(AllCharacters, key);

        // Act
        var decrypted = StringEncryptor.Decrypt(encrypted, key);

        // Assert
        decrypted.Should().Be(AllCharacters);
    }

    [DataTestMethod]
    [DynamicData(nameof(GetTestKeys), DynamicDataSourceType.Method)]
    public void Decrypt_Returns_Empty_String_If_Value_Is_Empty(string key)
    {
        var result = StringEncryptor.Decrypt(string.Empty, key);

        result.Should().BeNullOrEmpty();
    }

    [DataTestMethod]
    [DynamicData(nameof(GetTestKeys), DynamicDataSourceType.Method)]
    public void Encrypt_Result_Is_Not_Null_Or_Whitespace(string key)
    {
        var value = "This is a test";

        var encrypted = StringEncryptor.Encrypt(value, key);

        encrypted.Should().NotBeNullOrWhiteSpace(encrypted);
    }

    [DataTestMethod]
    [DynamicData(nameof(GetTestKeys), DynamicDataSourceType.Method)]
    public void Encrypt_Returns_Empty_String_If_Value_Is_Empty(string key)
    {
        var result = StringEncryptor.Encrypt(string.Empty, key);

        result.Should().BeNullOrEmpty();
    }

    #endregion
}