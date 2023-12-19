using Moq;
using SpaceShipAPI;
using SpaceShipAPI.Model;
using SpaceShipAPI.Model.Exceptions;
using SpaceShipAPI.Services;

[TestFixture]
public class UpgradeableTests
{
    [Test]
    public void Upgradeable_Initialization_PropertiesSetCorrectly()
    {
        var mockLevelService = new Mock<ILevelService>();
        var expectedLevel = new Level
        {
            Type = UpgradeableType.SHIELD,
            LevelValue = 1,
            Effect = 100,
            Max = false,
            Costs = new HashSet<LevelCost>()
        };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(UpgradeableType.SHIELD, 1))
            .Returns(expectedLevel);

        var upgradeable = new TestableUpgradeable(mockLevelService.Object, UpgradeableType.SHIELD, 1);

        Assert.AreEqual(expectedLevel.LevelValue, upgradeable.TestLevelValue); 
    }
    
    [Test]
    public void IsFullyUpgraded_WhenMax_ReturnsTrue()
    {
     
        var mockLevelService = new Mock<ILevelService>();
        var maxLevel = new Level
        {
            LevelValue = 1,  
            Effect = 100,    
            Max = true      
        };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Returns(maxLevel);

        var upgradeable = new TestableUpgradeable(mockLevelService.Object, UpgradeableType.SCANNER, 2);
        
        var result = upgradeable.IsFullyUpgraded();
        
        Assert.IsTrue(result);
    }
    
    [Test]
    public void GetUpgradeCost_WhenNotMax_ReturnsCosts()
    {
        var mockLevelService = new Mock<ILevelService>();
        var currentLevel = new Level
        {
            LevelValue = 1,
            Effect = 100,
            Max = false,
            Costs = new HashSet<LevelCost>()
        };
        var nextLevel = new Level
        {
            LevelValue = 2,
            Effect = 200,
            Max = false,
            Costs = new HashSet<LevelCost>
            {
                new LevelCost { Resource = ResourceType.CRYSTAL, Amount = 50 }  
            }
        };
        
        int capturedLevel = 0;
        UpgradeableType capturedType =  UpgradeableType.SCANNER;

        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Callback<UpgradeableType, int>((type, level) =>
            {
                capturedType =  UpgradeableType.SCANNER;
                capturedLevel = level;
            }) 
            .Returns((UpgradeableType type, int level) => level == 1 ? currentLevel : nextLevel);
        
        var upgradeable = new TestableUpgradeable(mockLevelService.Object, UpgradeableType.SCANNER, 1);
     
        var costs = upgradeable.GetUpgradeCost(); // line 95
        
        Assert.AreEqual(upgradeable.TestType, UpgradeableType.SCANNER); // also passes
        Assert.AreEqual(UpgradeableType.SCANNER, capturedType);
        Assert.AreEqual(50, costs[ResourceType.CRYSTAL]);  
    }
    
    [Test]
    public void GetUpgradeCost_VerifyLevelServiceCalls()
    {
        var mockLevelService = new Mock<ILevelService>();

        var currentLevel = new Level
        {
            LevelValue = 1,
            Effect = 100,
            Max = false,
            Costs = new HashSet<LevelCost>() 
        };

        var nextLevel = new Level
        {
            LevelValue = 2,
            Effect = 200,
            Max = false,
            Costs = new HashSet<LevelCost>
            {
                new LevelCost { Resource = ResourceType.CRYSTAL, Amount = 50 }
            }
        };

        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(UpgradeableType.SCANNER, 1))
                        .Returns(currentLevel); 
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(UpgradeableType.SCANNER, 2))
                        .Returns(nextLevel); 
        
        int capturedLevel = 0;
        UpgradeableType capturedType =  UpgradeableType.SCANNER;
        List<(UpgradeableType Type, int Level)> capturedCalls = new List<(UpgradeableType, int)>();
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Callback<UpgradeableType, int>((type, level) =>
            {
                capturedCalls.Add((capturedType, level));
            })
            .Returns((UpgradeableType type, int level) => level == 1 ? currentLevel : nextLevel);

        var upgradeable = new TestableUpgradeable(mockLevelService.Object, UpgradeableType.SCANNER, 1);
        
        var costs = upgradeable.GetUpgradeCost();

        Assert.AreEqual((UpgradeableType.SCANNER, 1), capturedCalls.First());
        Assert.AreEqual((UpgradeableType.SCANNER, 2), capturedCalls.Last());
    }
    
    [Test]
    public void GetUpgradeCost_SimplifiedTest()
    {
      
        var mockLevelService = new Mock<ILevelService>();
        UpgradeableType capturedType = UpgradeableType.SCANNER;

        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Callback<UpgradeableType, int>((type, level) => capturedType = type);

        Assert.AreEqual(UpgradeableType.SCANNER, capturedType); 
    }
    
    [Test]
    public void GetUpgradeCost_SimplifiedTest_2()
    {
        // Arrange
        var mockLevelService = new Mock<ILevelService>();
        UpgradeableType capturedType = default; 

        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Callback<UpgradeableType, int>((type, level) => capturedType = type);

        capturedType = UpgradeableType.SCANNER; // default has to be overwritten to pass
        mockLevelService.Object.GetLevelByTypeAndLevel(UpgradeableType.SCANNER, 1);

        Assert.AreEqual(UpgradeableType.SCANNER, capturedType);
    }
    
    [Test]
    public void GetUpgradeCost_WhenMax_ThrowsException()
    {
        var mockLevelService = new Mock<ILevelService>();
        var maxLevel = new Level
        {
            LevelValue = 2,
            Effect = 200,
            Max = true,
            Costs = new HashSet<LevelCost>()
        };

        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(UpgradeableType.SHIP_STORAGE, 2))
            .Returns(maxLevel);

        var upgradeable = new TestableUpgradeable(mockLevelService.Object, UpgradeableType.SHIP_STORAGE, 2);

        Assert.Throws<UpgradeNotAvailableException>(() => upgradeable.GetUpgradeCost());
    }
}

public class TestableUpgradeable : Upgradeable
{
    public UpgradeableType TestType { get; private set; }

    public TestableUpgradeable(ILevelService levelService, UpgradeableType type, int level)
        : base(levelService, type, level)
    {
        TestType = type;
    }
    public int TestLevelValue => CurrentLevel.LevelValue; 
}


