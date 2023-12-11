using Moq;
using SpaceShipAPI;
using SpaceShipAPI.Model;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Repository;
using SpaceShipAPI.Services;

namespace TestProject;

[TestFixture]
public class MinerShipTest
{

    [Test]
    public void CreateNewMinerShip_ReturnsMinerShipWithInitialSettings()
    {
        var mockLevelService = new Mock<ILevelService>();
    
        // Mock the LevelService to return a Level object when GetLevelByTypeAndLevel is called
        var shieldLevel = new Level { LevelValue = 1, Effect = 100, Max = false };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(UpgradeableType.SHIELD, 1))
            .Returns(shieldLevel);

        // Now create the ship
        var ship = MinerShipManager.CreateNewMinerShip(mockLevelService.Object, "TestShip", ShipColor.RED);

        Assert.AreEqual("TestShip", ship.Name);
        Assert.AreEqual(ShipColor.RED, ship.Color);
        Assert.AreEqual(1, ship.EngineLevel);
        Assert.AreEqual(1, ship.ShieldLevel);
        Assert.AreEqual(shieldLevel.Effect, ship.ShieldEnergy); // Asserting ShieldEnergy is set correctly
        // Additional assertions for other initial settings
    }
    
    [Test]
    public void GetDetailedDTO_ReturnsCorrectMinerShipDTO()
    {
        // Arrange
        var mockLevelService = new Mock<ILevelService>();

        // Setup mockLevelService to return Level objects for each part
        var level = new Level { LevelValue = 1, Effect = 100, Max = false };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Returns(level);

        // Initialize a MinerShip with test data
        var minerShip = new MinerShip
        {
            Id = 123,
            Name = "TestShip",
            Color = ShipColor.RED,
            EngineLevel = 1,
            ShieldLevel = 1,
            DrillLevel = 1,
            StorageLevel = 1,
            CurrentMission = new MiningMission(),
            StoredResources =new List<StoredResource>()
            {
                new StoredResource(),
            }
        };

        // Create MinerShipManager with the mockLevelService and the test MinerShip
        var minerShipManager = new MinerShipManager(mockLevelService.Object, minerShip);
     
        // Act
        var dto = minerShipManager.GetDetailedDTO();

        // Assert
        Assert.IsNotNull(dto);
        Assert.IsInstanceOf<MinerShipDTO>(dto);
        // Additional assertions to check if the DTO is correctly populated
    }
    
    [Test]
    public void GetUpgradeCost_ReturnsCorrectCostForPart()
    {
        // Arrange
        // Arrange
        var mockLevelService = new Mock<ILevelService>();

        // Setup mockLevelService to return Level objects for each part
        var currentLevelCosts = new List<LevelCost>
        {
            new LevelCost { Resource = ResourceType.CRYSTAL, Amount = 50 }
        };
        var nextLevelCosts = new List<LevelCost>
        {
            new LevelCost { Resource = ResourceType.CRYSTAL, Amount = 100 }
        };

        var currentLevel = new Level { LevelValue = 1, Effect = 100, Max = false, Costs = currentLevelCosts };
        var nextLevel = new Level { LevelValue = 2, Effect = 200, Max = false, Costs = nextLevelCosts };

        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), 1)).Returns(currentLevel);
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), 2)).Returns(nextLevel);
        
        // Initialize a MinerShip with test data
        var minerShip = new MinerShip
        {
            EngineLevel = 1, // Initial engine level
            ShieldLevel = 1, // Initial shield level
            // Initialize other properties if needed
        };

        // Create MinerShipManager with the mockLevelService and the test MinerShip
        var minerShipManager = new MinerShipManager(mockLevelService.Object, minerShip);

        // Act
        var costs = minerShipManager.GetUpgradeCost(ShipPart.ENGINE);

        // Assert
        Assert.IsNotNull(costs);
        Assert.AreEqual(100, costs[ResourceType.CRYSTAL]); // Verify the cost is as expected for the next level
    }

    [Test]
    public void UpgradePart_UpgradesSpecifiedPart()
    {
        // Arrange
        var mockLevelService = new Mock<ILevelService>();

        // Setup mockLevelService to return Level objects for each part
        var levelBeforeUpgrade = new Level { LevelValue = 1, Effect = 100, Max = false };
        var levelAfterUpgrade = new Level { LevelValue = 2, Effect = 200, Max = false };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), 1))
            .Returns(levelBeforeUpgrade);
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), 2))
            .Returns(levelAfterUpgrade);

        // Initialize a MinerShip with test data
        var minerShip = new MinerShip
        {
            EngineLevel = 1, // Initial engine level
            ShieldLevel = 1, // Initial shield level
            // Initialize other properties if needed
        };

        // Create MinerShipManager with the mockLevelService and the test MinerShip
        var minerShipManager = new MinerShipManager(mockLevelService.Object, minerShip);

        // Act
        Assert.DoesNotThrow(() => minerShipManager.UpgradePart(ShipPart.ENGINE));

        // Assert
        Assert.AreEqual(2, minerShip.EngineLevel); // Engine level should be upgraded
        // Optionally, add assertions for other parts if they are affected
    }

}


