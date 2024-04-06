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
        private MapService _mapService;

        protected override void OnSetup()
        {
            _mapService = CreateInstance<MapService>();
        }

        [TestMethod]
        public void RefreshAvailableMaps_Should_Refresh_Maps()
        {
            // Arrange
            var customMaps = new List<string> { "CustomMap1", "CustomMap2" };
            var fileSystemServiceMock = GetMock<IFileSystemService>();

            fileSystemServiceMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            fileSystemServiceMock.Setup(fs => fs.ReadAllLines(It.IsAny<string>())).Returns(customMaps);

            // Act
            _mapService.RefreshAvailableMaps();

            // Assert
            fileSystemServiceMock.Verify(fs => fs.FileExists(It.IsAny<string>()), Times.Once);
            fileSystemServiceMock.Verify(fs => fs.ReadAllLines(It.IsAny<string>()), Times.Once);

            _mapService.CustomIDs.Should().BeEquivalentTo(customMaps);
        }

        // Add more tests...
    }
}