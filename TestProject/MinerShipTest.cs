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
    
        var shieldLevel = new Level { LevelValue = 1, Effect = 100, Max = false };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(UpgradeableType.SHIELD, 1))
            .Returns(shieldLevel);
        
        var ship = MinerShipManager.CreateNewMinerShip(mockLevelService.Object, "TestShip", ShipColor.RED);

        Assert.AreEqual("TestShip", ship.Name);
        Assert.AreEqual(ShipColor.RED, ship.Color);
        Assert.AreEqual(1, ship.EngineLevel);
        Assert.AreEqual(1, ship.ShieldLevel);
        Assert.AreEqual(shieldLevel.Effect, ship.ShieldEnergy);
    }
    
    [Test]
    public void GetDetailedDTO_ReturnsCorrectMinerShipDTO()
    {
        var mockLevelService = new Mock<ILevelService>();

        var level = new Level { LevelValue = 1, Effect = 100, Max = false };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Returns(level);

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

        var minerShipManager = new MinerShipManager(mockLevelService.Object, minerShip);
     
        var dto = minerShipManager.GetDetailedDTO();

        Assert.IsNotNull(dto);
        Assert.IsInstanceOf<MinerShipDTO>(dto);
    }
    
    [Test]
    public void GetUpgradeCost_ReturnsCorrectCostForPart()
    {
        var mockLevelService = new Mock<ILevelService>();

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
        
        var minerShip = new MinerShip
        {
            EngineLevel = 1, 
            ShieldLevel = 1, 
        };

        var minerShipManager = new MinerShipManager(mockLevelService.Object, minerShip);

        var costs = minerShipManager.GetUpgradeCost(ShipPart.ENGINE);

        Assert.IsNotNull(costs);
        Assert.AreEqual(100, costs[ResourceType.CRYSTAL]); // Verify the cost is as expected for the next level
    }

    [Test]
    public void UpgradePart_UpgradesSpecifiedPart()
    {
        var mockLevelService = new Mock<ILevelService>();

        var levelBeforeUpgrade = new Level { LevelValue = 1, Effect = 100, Max = false };
        var levelAfterUpgrade = new Level { LevelValue = 2, Effect = 200, Max = false };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), 1))
            .Returns(levelBeforeUpgrade);
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), 2))
            .Returns(levelAfterUpgrade);

        var minerShip = new MinerShip
        {
            EngineLevel = 1, 
            ShieldLevel = 1,
        };

        var minerShipManager = new MinerShipManager(mockLevelService.Object, minerShip);
        
        Assert.DoesNotThrow(() => minerShipManager.UpgradePart(ShipPart.ENGINE));
        
        Assert.AreEqual(2, minerShip.EngineLevel); 
    }

}


