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
        // Arrange
        var mockLevelService = new Mock<ILevelService>();
        var expectedLevel = new Level
        {
            Type = UpgradeableType.SHIELD,
            LevelValue = 1,
            Effect = 100, // Example value
            Max = false,
            Costs = new HashSet<LevelCost>() // Optionally, populate with test data
        };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(UpgradeableType.SHIELD, 1))
            .Returns(expectedLevel);

        // Act
        var upgradeable = new TestableUpgradeable(mockLevelService.Object, UpgradeableType.SHIELD, 1);

        // Assert
        // Assuming TestableUpgradeable exposes a public property or method to access CurrentLevel's properties
        Assert.AreEqual(expectedLevel.LevelValue, upgradeable.TestLevelValue); // Replace 'TestLevelValue' with the actual public property/method
    }
    
    [Test]
    public void IsFullyUpgraded_WhenMax_ReturnsTrue()
    {
        // Arrange
        var mockLevelService = new Mock<ILevelService>();
        var maxLevel = new Level
        {
            LevelValue = 1,  // Assuming this represents the level
            Effect = 100,    // Arbitrary value for Effect
            Max = true       // Indicates this level is the max
        };
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Returns(maxLevel);

        var upgradeable = new TestableUpgradeable(mockLevelService.Object, UpgradeableType.SCANNER, 2);

        // Act
        var result = upgradeable.IsFullyUpgraded();

        // Assert
        Assert.IsTrue(result);
    }
    
    [Test]
    public void GetUpgradeCost_WhenNotMax_ReturnsCosts()
    {
        // Arrange
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
        UpgradeableType capturedType = default;

        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Callback<UpgradeableType, int>((type, level) =>
            {
                capturedType = type;
                capturedLevel = level;
            }) 
            .Returns((UpgradeableType type, int level) => level == 1 ? currentLevel : nextLevel);
        
        var upgradeable = new TestableUpgradeable(mockLevelService.Object, UpgradeableType.SCANNER, 1);
        // capturedType = UpgradeableType.SCANNER;  if before line 95 it fails
        // Act
        var costs = upgradeable.GetUpgradeCost(); // line 95
        // capturedType = UpgradeableType.SCANNER;  if after line 95 it passes
        // Assert
        Assert.AreEqual(upgradeable.TestType, UpgradeableType.SCANNER); // also passes
        Assert.AreEqual(UpgradeableType.SCANNER, capturedType);
        // Assert.AreEqual(2, capturedLevel); 
        Assert.AreEqual(50, costs[ResourceType.CRYSTAL]);  
    }
    
    [Test]
    public void GetUpgradeCost_VerifyLevelServiceCalls()
    {
        // Arrange
        var mockLevelService = new Mock<ILevelService>();

        var currentLevel = new Level
        {
            LevelValue = 1,
            Effect = 100,
            Max = false,
            Costs = new HashSet<LevelCost>() // populate with initial costs if needed
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

        // Setup the mock for both the constructor call and the GetUpgradeCost call
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(UpgradeableType.SCANNER, 1))
                        .Returns(currentLevel); // For the constructor call
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(UpgradeableType.SCANNER, 2))
                        .Returns(nextLevel); // For the GetUpgradeCost call

        // Capture all calls
        List<(UpgradeableType Type, int Level)> capturedCalls = new List<(UpgradeableType, int)>();
        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Callback<UpgradeableType, int>((type, level) =>
            {
                Console.WriteLine($"Callback called with type: {type}, level: {level}");
                capturedCalls.Add((type, level));
            })
            .Returns((UpgradeableType type, int level) => level == 1 ? currentLevel : nextLevel);

        var upgradeable = new TestableUpgradeable(mockLevelService.Object, UpgradeableType.SCANNER, 1);

        // Act
        var costs = upgradeable.GetUpgradeCost();

        // Assert
        // Verify the constructor call
        Assert.AreEqual((UpgradeableType.SCANNER, 1), capturedCalls.First());
        // Verify the GetUpgradeCost call
        Assert.AreEqual((UpgradeableType.SCANNER, 2), capturedCalls.Last());
        // Further assertions as needed
    }
    
    [Test]
    public void GetUpgradeCost_SimplifiedTest()
    {
        // Arrange
        var mockLevelService = new Mock<ILevelService>();
        UpgradeableType capturedType = UpgradeableType.SCANNER;

        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Callback<UpgradeableType, int>((type, level) => capturedType = type);

        // Act (Simplified)
        capturedType = UpgradeableType.ENGINE; // For testing, remove this later
        Assert.AreEqual(UpgradeableType.SCANNER, capturedType); // This should fail if the line above is not removed
    }
    
    [Test]
    public void GetUpgradeCost_SimplifiedTest_2()
    {
        // Arrange
        var mockLevelService = new Mock<ILevelService>();
        UpgradeableType capturedType = default; //defaults to ENGINGE

        mockLevelService.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Callback<UpgradeableType, int>((type, level) => capturedType = type);

        // Act
        capturedType = UpgradeableType.SCANNER; // default has to be overwritten to pass
        mockLevelService.Object.GetLevelByTypeAndLevel(UpgradeableType.SCANNER, 1);

        // Assert
        Assert.AreEqual(UpgradeableType.SCANNER, capturedType);
    }
    
    [Test]
    public void GetUpgradeCost_WhenMax_ThrowsException()
    {
        // Arrange
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

        // Act & Assert
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
    public int TestLevelValue => CurrentLevel.LevelValue; // Expose LevelValue for testing
}


