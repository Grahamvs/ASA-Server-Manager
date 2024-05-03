using ASA_Server_Manager.Configs;
using ASA_Server_Manager.Interfaces.Serialization;
using ASA_Server_Manager.Interfaces.Services;
using ASA_Server_Manager.Services;
using ASA_Server_Manager_Tests.Common;
using FluentAssertions;
using Moq;

namespace ASA_Server_Manager_Tests.Services
{
    [TestClass]
    public class MapServiceTests : BaseTestWithContainer
    {
        #region Private Fields

        private MapService _mapService;

        #endregion

        #region Setup

        protected override void OnSetup()
        {
            _mapService = CreateInstance<MapService>();
        }

        #endregion

        #region Tests

        [TestMethod]
        public void AvailableMaps_Should_Return_Combination_Of_Official_And_Custom_Maps()
        {
            // Arrange
            var customMaps = new List<MapDetails>
            {
                new() {ID = "CustomMap1"},
                new() {ID = "CustomMap2"},
            };

            _mapService.SetCustomMaps(customMaps);

            var expectedMaps = _mapService.OfficialMaps.Concat(customMaps);

            // Act
            var availableMaps = _mapService.AvailableMaps;

            // Assert
            availableMaps.Should().BeEquivalentTo(expectedMaps);
        }

        [TestMethod]
        public void RefreshAvailableMaps_Should_Refresh_Maps()
        {
            // Arrange
            var customMaps = new List<MapDetails>
            {
                new() {ID = "CustomMap1"},
                new() {ID = "CustomMap2"},
            };

            var fileSystemServiceMock = GetMock<IFileSystemService>();
            var serializer = GetMock<ISerializer>();

            fileSystemServiceMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            serializer.Setup(s => s.DeserializeFromFile<List<MapDetails>>(It.IsAny<string>())).Returns(customMaps);

            // Act
            _mapService.RefreshAvailableMaps();

            // Assert
            fileSystemServiceMock.Verify(fs => fs.FileExists(It.IsAny<string>()), Times.Once);

            _mapService.CustomMaps.Should().BeEquivalentTo(customMaps);
        }

        [TestMethod]
        public void Save_Should_Serialize_CustomMaps_To_File()
        {
            // Arrange
            var customMaps = new List<MapDetails>
            {
                new() {ID = "CustomMap1"},
                new() {ID = "CustomMap2"},
            };

            _mapService.SetCustomMaps(customMaps);

            var fileSystemServiceMock = GetMock<IFileSystemService>();
            var serializerMock = GetMock<ISerializer>();

            // Act
            _mapService.Save();

            // Assert
            serializerMock.Verify(s => s.SerializeToFile(It.Is<List<MapDetails>>(maps => maps.SequenceEqual(customMaps)), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void SetCustomMaps_Should_Set_CustomMaps()
        {
            // Arrange
            var customMaps = new List<MapDetails>
            {
                new() {ID = "CustomMap1"},
                new() {ID = "CustomMap2"},
            };

            // Act
            _mapService.SetCustomMaps(customMaps);

            // Assert
            _mapService.CustomMaps.Should().BeEquivalentTo(customMaps);
        }

        #endregion
    }
}