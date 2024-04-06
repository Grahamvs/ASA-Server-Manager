using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Enums;
using ASA_Server_Manager.Interfaces.Serialization;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Services;
using ASA_Server_Manager_Tests.Common;
using FluentAssertions;
using Moq;

namespace ASA_Server_Manager_Tests.Services
{
    [TestClass]
    public class ServerProfileServiceTests : BaseTestWithContainer
    {
        #region Private Fields

        private readonly string _profilePath = "testProfilePath";
        private List<string> _officialMaps;
        private ServerProfileService _sut;

        #endregion

        #region Setup

        protected override void OnSetup()
        {
            _officialMaps = [.. new[] { "Map1" }];

            GetMock<IMapService>()
                .Setup(service => service.OfficialIDs)
                .Returns(_officialMaps);

            _sut = CreateInstance<ServerProfileService>();
        }

        #endregion

        #region Tests

        [TestMethod]
        public void LoadLastProfile_Should_Load_The_Last_Used_Profile_If_Its_Not_Null()
        {
            // Arrange
            var defaultProfile = new ServerProfile { Map = "DefaultMap", Port = 1234 };
            var appSettingsService = GetMock<IAppSettingsService>();
            var fileSystemServiceMock = GetMock<IFileSystemService>();
            var serializationServiceMock = GetMock<ISerializer>();

            var testProfilePath = "Test Profile";

            appSettingsService.Setup(service => service.LastProfile).Returns(testProfilePath);
            fileSystemServiceMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            serializationServiceMock.Setup(ss => ss.DeserializeFromFile<ServerProfile>(It.IsAny<string>())).Returns(defaultProfile);

            // Act
            _sut.LoadLastProfile();

            // Assert
            fileSystemServiceMock.Verify(fs => fs.FileExists(testProfilePath), Times.Once);
            serializationServiceMock.Verify(ss => ss.DeserializeFromFile<ServerProfile>(testProfilePath), Times.Once);
            Assert.AreEqual(defaultProfile, _sut.CurrentProfile);
        }

        [TestMethod]
        public void LoadProfile_Should_Load_From_File()
        {
            // Arrange
            var profile = new ServerProfile { Map = "TestMap", Port = 1234 };
            var fileSystemServiceMock = GetMock<IFileSystemService>();
            var serializationServiceMock = GetMock<ISerializer>();

            fileSystemServiceMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            serializationServiceMock.Setup(ss => ss.DeserializeFromFile<ServerProfile>(It.IsAny<string>())).Returns(profile);

            // Act
            _sut.LoadProfile(_profilePath);

            // Assert
            fileSystemServiceMock.Verify(fs => fs.FileExists(_profilePath), Times.Once);
            serializationServiceMock.Verify(ss => ss.DeserializeFromFile<ServerProfile>(_profilePath), Times.Once);

            profile
                .Should()
                .BeEquivalentTo(_sut.CurrentProfile);
        }

        [TestMethod]
        public void ResetProfile_ShouldResetToDefaultConfiguration()
        {
            // Arrange
            var defaultProfile = _sut.CreateDefaultProfile();
            _sut.LoadLastProfile();

            var profile = (ServerProfile)_sut.CurrentProfile;

            profile.Map = "NewMap";
            profile.Port = 5678;
            profile.ServerPassword = "NewPassword";
            profile.AdminPassword = "NewAdminPassword";
            profile.SaveDirectoryName = "NewSaveDirectory";

            profile.SelectedModIds = new Dictionary<int, ModMode>
            {
                {123, ModMode.Enabled},
                {456, ModMode.Passive},
            };

            // Act
            _sut.ResetProfile();

            // Assert
            _sut.CurrentProfile.Should().NotBeEquivalentTo(profile);
            _sut.CurrentProfile.Should().BeEquivalentTo(defaultProfile);
        }

        [TestMethod]
        public void SaveConfiguration_ShouldSaveCurrentConfigurationToFile()
        {
            // Arrange
            var serializationServiceMock = GetMock<ISerializer>();
            var profile = (ServerProfile)_sut.CurrentProfile;

            profile.Map = "NewMap";
            profile.Port = 5678;
            profile.ServerPassword = "NewPassword";
            profile.AdminPassword = "NewAdminPassword";
            profile.SaveDirectoryName = "NewSaveDirectory";

            profile.SelectedModIds = new Dictionary<int, ModMode>
            {
                {123, ModMode.Enabled},
                {456, ModMode.Passive},
            };

            // Act
            var testProfilePath = "testProfilePath";
            _sut.SaveProfile(testProfilePath);

            // Assert
            serializationServiceMock.Verify(ss => ss.SerializeToFile(profile, testProfilePath), Times.Once);
        }

        #endregion

        // Add more tests...
    }
}