using ASA_Server_Manager.Interfaces.Serialization;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager_Tests.Common;
using ASA_Server_Manager_Tests.Serialization.HelperClasses;
using FluentAssertions;
using Moq;

namespace ASA_Server_Manager_Tests.Serialization;

[TestClass]
public abstract class BaseSerializationServiceTests<TImplementation> : BaseTestWithContainer
where TImplementation : class, ISerializer
{
    #region Private Fields

    private ISerializer _sut;
    private const string FilePath = "test file";

    #endregion

    #region Setup

    protected override void OnSetup()
    {
        _sut = CreateInstance<TImplementation>();
    }

    protected abstract string GetSerializeTestClassString(string property1, string property2, string property3);

    protected abstract string GetSerializeTestClassWithIgnoreString(string property1, string property2);

    #endregion

    #region Tests

    [TestMethod]
    public void Deserialize_Should_Deserialize_Object()
    {
        // Arrange
        var prop1 = "Test1";
        var prop2 = "Test2";
        var prop3 = "Test3";

        var serializedText = GetSerializeTestClassString(prop1, prop2, prop3);

        // Act
        var result = _sut.Deserialize<SerializeTestClass>(serializedText);

        // Assert
        result.Should().NotBeNull();
        result.Property1.Should().Be(prop1);
        result.Property2.Should().Be(prop2);
        result.Property3.Should().Be(prop3);
    }

    [TestMethod]
    public void DeserializeFromFile_Should_Deserialize_Object_From_File()
    {
        // Arrange
        var prop1 = "Test1";
        var prop2 = "Test2";
        var prop3 = "Test3";

        var serializedText = GetSerializeTestClassString(prop1, prop2, prop3);

        var fileSystemServiceMock = GetMock<IFileSystemService>();
        fileSystemServiceMock.Setup(fs => fs.ReadAllText(FilePath)).Returns(serializedText);

        // Act
        var result = _sut.DeserializeFromFile<SerializeTestClass>(FilePath);

        // Assert
        fileSystemServiceMock.Verify(fs => fs.ReadAllText(FilePath), Times.Once);

        result.Should().NotBeNull();
        result.Property1.Should().Be(prop1);
        result.Property2.Should().Be(prop2);
        result.Property3.Should().Be(prop3);
    }

    [TestMethod]
    public void Serialize_Should_Ignore_Properties_With_DoNotSerialize_Attribute()
    {
        // Arrange
        var prop1 = "Test1";
        var prop2 = "Test2";
        var secretProp = "Secret";

        var testObject = new SerializeTestClassWithIgnore { Property1 = prop1, Property2 = prop2, SecretProperty = secretProp };

        var expected = GetSerializeTestClassWithIgnoreString(prop1, prop2);

        // Act
        var result = _sut.Serialize(testObject);

        // Assert
        result.Should().Be(expected);
        result.Should().Contain(prop1);
        result.Should().Contain(prop2);
        result.Should().NotContain(secretProp);
    }

    [TestMethod]
    public void Serialize_Should_Serialize_Object()
    {
        // Arrange
        var prop1 = "Test1";
        var prop2 = "Test2";
        var prop3 = "Test3";

        var serializedText = GetSerializeTestClassString(prop1, prop2, prop3);

        var testObject = new SerializeTestClass { Property1 = prop1, Property2 = prop2, Property3 = prop3 };

        // Act
        var result = _sut.Serialize(testObject);

        // Assert
        result.Should().Be(serializedText);
    }

    [TestMethod]
    public void SerializeToFile_Should_Write_Serialized_Value_To_File()
    {
        // Arrange
        var testObject = new SerializeTestClass() { Property1 = "test 1", Property2 = "test 2", Property3 = "test 3" };

        // Act
        _sut.SerializeToFile(testObject, FilePath);

        // Assert
        GetMock<IFileSystemService>().Verify(fs => fs.WriteAllText(FilePath, It.IsAny<string>()), Times.Once);
    }

    #endregion
}