using Moq;
using SpaceShipAPI;
using SpaceShipAPI.Model;
using SpaceShipAPI.Model.Exceptions;
using SpaceShipAPI.Repository;
using SpaceShipAPI.Services;

namespace TestProject;

[TestFixture]
public class UpgradeableTests
{
    public class TestableStorageManager : AbstractStorageManager
    {
        public TestableStorageManager(ILevelService levelService, UpgradeableType type, int level,
            ICollection<StoredResource> storedResources)
            : base(levelService, type, level, storedResources)
        {
        }
    }

    [Test]
    public void Constructor_WithExcessiveResources_ThrowsException()
    {
        var mockLevelService = new Mock<ILevelService>();
        var level = new Level { LevelValue = 1, Effect = 100, Max = false };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Returns(level);

        var storedResources = new List<StoredResource>()
        {
            new StoredResource { ResourceType = ResourceType.PLUTONIUM, Amount = 120 },
            new StoredResource { ResourceType = ResourceType.CRYSTAL, Amount = 120 },
        };

        Assert.Throws<ResourceCapacityExceededException>(() =>
                new TestableStorageManager(mockLevelService.Object, UpgradeableType.SHIP_STORAGE, 1, storedResources),
            "Expected ResourceCapacityExceededException for exceeding storage capacity");
    }
    
    [Test]
    public void AddResource_SuccessfullyAddsResources()
    {
        var mockLevelService = new Mock<ILevelService>();
        var level = new Level { LevelValue = 1, Effect = 100, Max = false };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Returns(level);

        var storedResources = new List<StoredResource>()
        {
            new StoredResource { ResourceType = ResourceType.PLUTONIUM, Amount = 0 },
            new StoredResource { ResourceType = ResourceType.CRYSTAL, Amount = 0 },
        };
        var storageManager = new TestableStorageManager(mockLevelService.Object, UpgradeableType.SHIP_STORAGE, 1, storedResources);
        bool result = storageManager.AddResource(ResourceType.CRYSTAL, 60);

        Assert.IsTrue(result, "Resource should be added successfully");
        var resource = storageManager.GetStoredResources().First(resource => resource.ResourceType == ResourceType.CRYSTAL);
        Assert.That(resource.Amount, Is.EqualTo(60));
    }
    
    [Test]
    public void AddResource_ExceedingCapacity_ThrowsException()
    {
        // Arrange
        var mockLevelService = new Mock<ILevelService>();
        var level = new Level { LevelValue = 1, Effect = 100, Max = true }; 
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>())).Returns(level);

        var storedResources = new List<StoredResource>()
        {
            new StoredResource { ResourceType = ResourceType.PLUTONIUM, Amount = 0 },
            new StoredResource { ResourceType = ResourceType.CRYSTAL, Amount = 0 },
        };
        var storageManager = new TestableStorageManager(mockLevelService.Object, UpgradeableType.SHIP_STORAGE, 1, storedResources);

        // Act & Assert
        Assert.Throws<InsufficientStorageSpaceException>(() =>
                storageManager.AddResource(ResourceType.CRYSTAL, 101), 
            "Expected InsufficientStorageSpaceException for exceeding storage capacity");    }

}   


