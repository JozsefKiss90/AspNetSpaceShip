using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Moq;
using SpaceshipAPI;
using SpaceShipAPI;
using SpaceShipAPI.Model;
using SpaceShipAPI.Model.DTO;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Services;
using SpaceshipAPI.Spaceship.Model.Station;

[TestFixture]
public class SpaceStationServiceTests
{
    private Mock<SpaceStation> _spaceStationMock;
    private Mock<UserManager<UserEntity>> _userManagerMock;
    private Mock<ISpaceStationRepository> _spaceStationRepositoryMock;
    private Mock<ISpaceStationManager> _spaceStationManagerMock; 
    private Mock<IHangarManager> _hangarManagerMock; 
    private SpaceStationService _spaceStationService;
    private Mock<ILevelService> _levelServiceMock;
    private Mock<ISpaceStationService> _spaceStationServiceMock;
    private SpaceStation _spaceStation;
    [SetUp]
    public void SetUp()
    {
        _spaceStation = new SpaceStation();
        _spaceStationMock = new Mock<SpaceStation>();
        _userManagerMock = IdentityMocks.MockUserManager<UserEntity>();
        _spaceStationRepositoryMock = new Mock<ISpaceStationRepository>(MockBehavior.Strict);
        _spaceStationManagerMock = new Mock<ISpaceStationManager>(MockBehavior.Strict);
        _hangarManagerMock = new Mock<IHangarManager>(MockBehavior.Strict); 
        _levelServiceMock = new Mock<ILevelService>(MockBehavior.Strict);
        _spaceStationServiceMock = new Mock<ISpaceStationService>(MockBehavior.Strict);
        
        _hangarManagerMock.Setup(hangar => hangar.GetCurrentCapacity()).Returns(10); 
        _hangarManagerMock.Setup(hangar => hangar.GetCurrentAvailableDocks()).Returns(10); 
        _hangarManagerMock.Setup(hangar => hangar.GetAllShips()).Returns(new HashSet<SpaceShip>());
      
        _levelServiceMock.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
            .Returns(new Level
            {
                Id = 1, 
                Type = UpgradeableType.ENGINE, 
                LevelValue = 1, 
                Effect = 10, 
                Max = false,
                Costs = new List<LevelCost>
                {
                    new LevelCost
                    {
                        Id = 1, 
                        Resource = ResourceType.METAL, 
                        Amount = 5 
                    },
                }
            });

        _spaceStationManagerMock.Setup(manager => manager.CreateHangarIfNotExists(_spaceStation)).Callback(() => { });

        _spaceStationService = new SpaceStationService(
            _userManagerMock.Object,
            _spaceStationRepositoryMock.Object,
            new Mock<ISpaceShipRepository>().Object,
            new Mock<IShipManagerFactory>().Object,
            _levelServiceMock.Object,
            _spaceStationManagerMock.Object 
        );
    }

    [Test]
    public async Task CreateAsync_CreatesSpaceStation_ForValidInput()
    {
        // Arrange
        var userId = "test-user-id";
        var userName = "test-user";
        var stationName = "New Station";
        var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName)
        };
        var userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "mock"));
        var currentUser = new UserEntity { Id = userId };

        var createdSpaceStation = new SpaceStation
        {
            Id = 1,
            Name = stationName,
            User = currentUser,
            Hangar = new HashSet<SpaceShip>(), 
        };
        
        var mockHangarDTO = new HangarDTO(new HashSet<ShipDTO>(), 0, 0, 0, false);
        
        var mockStorageDTO = new SpaceStationStorageDTO(
            new Dictionary<ResourceType, int>
            {
                { ResourceType.METAL, 2 }
            },
            0,
            1,
            1,
            false
        );
        
        _userManagerMock.Setup(manager => manager.GetUserAsync(userClaimsPrincipal)).ReturnsAsync(currentUser);
        _spaceStationRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<SpaceStation>())).ReturnsAsync(createdSpaceStation);

        _spaceStationManagerMock.Setup(manager => manager.CreateNewSpaceStation(stationName))
            .Returns(createdSpaceStation);
        _spaceStationManagerMock.Setup(manager => manager.GetHangarDTO(_spaceStation))
            .Returns(mockHangarDTO);
        _spaceStationManagerMock.Setup(manager => manager.CreateHangarIfNotExists(_spaceStation)).Callback(() => {/* do nothing */});
        
        _spaceStationManagerMock.Setup(manager => manager.GetStorageDTO(_spaceStation))
            .Returns(mockStorageDTO);
        // Act
        var result = await _spaceStationService.CreateAsync(stationName, userClaimsPrincipal);
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(stationName, result.Name);
        Assert.AreEqual(1, result.Id);
        _spaceStationRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<SpaceStation>()), Times.Once);
        _userManagerMock.Verify(manager => manager.GetUserAsync(userClaimsPrincipal), Times.Once);
        _spaceStationManagerMock.Verify(manager => manager.CreateNewSpaceStation(stationName), Times.Once);
    }
    
    [Test]
    public async Task GetBaseByIdAsync_ReturnsSpaceStation_ForValidUserAndStationId()
    {
        // Arrange
        var stationId = 1L;
        var userId = "1";
        var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "mock"));
        var currentUser = new UserEntity { Id = userId };
        var spaceStation = new SpaceStation { Id = stationId, Name = "Test Station", User = currentUser,  StoredResources =  new List<StoredResource>() };

        // Set up mocks for GetStationByIdAndCheckAccessAsync dependencies
        _spaceStationRepositoryMock.Setup(repo => repo.GetByIdAsync(stationId)).ReturnsAsync(spaceStation);
        _userManagerMock.Setup(manager => manager.FindByIdAsync(userId)).ReturnsAsync(currentUser);

        var mockShips = new HashSet<ShipDTO>();
        var mockHangarDTO = new HangarDTO(mockShips, 1, 10, 10, false);
        var mockStorageDTO = new SpaceStationStorageDTO(
            new Dictionary<ResourceType, int>
            {
                { ResourceType.METAL, 2 }
            },
            0,
            1,
            1,
            false
        );        
        var mockSpaceStationDTO = new SpaceStationDTO(stationId, "Test Station", mockHangarDTO, mockStorageDTO);

        _spaceStationManagerMock.Setup(manager => manager.GetStationDTO(_spaceStation)).Returns(mockSpaceStationDTO);

        // Act
        var result = await _spaceStationService.GetBaseByIdAsync(stationId, userClaimsPrincipal);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(stationId, result.Id);
        Assert.AreEqual("Test Station", result.Name);
        // Verify that the necessary dependencies were called
        _spaceStationRepositoryMock.Verify(repo => repo.GetByIdAsync(stationId), Times.Once);
        _userManagerMock.Verify(manager => manager.FindByIdAsync(userId), Times.Once);
        _spaceStationManagerMock.Verify(manager => manager.GetStationDTO(_spaceStation), Times.Once);
    }
    
    [Test]
    public async Task GetStationByIdAndCheckAccessAsync_ReturnsStation_ForValidUserAndStationId()
    {
        // Arrange
        var stationId = 1L;
        var userId = "1";
        var userClaims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "mock"));
        var currentUser = new UserEntity { Id = userId };
        var spaceStation = new SpaceStation { Id = stationId, Name = "Test Station", User = currentUser };

        _spaceStationRepositoryMock.Setup(repo => repo.GetByIdAsync(stationId)).ReturnsAsync(spaceStation);
        _userManagerMock.Setup(manager => manager.FindByIdAsync(userId)).ReturnsAsync(currentUser); // Assuming GetCurrentUser uses FindByIdAsync

        // Act
        var result = await _spaceStationService.GetStationByIdAndCheckAccessAsync(stationId, userClaimsPrincipal);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(stationId, result.Id);
        _spaceStationRepositoryMock.Verify(repo => repo.GetByIdAsync(stationId), Times.Once);
        _userManagerMock.Verify(manager => manager.FindByIdAsync(userId), Times.Once);
    }
}
