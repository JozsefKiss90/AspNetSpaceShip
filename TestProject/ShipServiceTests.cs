using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Moq;
using SpaceshipAPI;
using SpaceShipAPI.Model;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Services;
using SpaceshipAPI.Spaceship.Model.Station;

namespace SpaceShipAPI.Tests.Services
{
    [TestFixture]
    public class ShipServiceTests
    {
        private ShipService _shipService;
        private Mock<ISpaceShipRepository> _spaceShipRepositoryMock;
        private Mock<ISpaceStationRepository> _spaceStationRepositoryMock;
        private Mock<IShipManagerFactory> _shipManagerFactoryMock;
        private Mock<IMissionFactory> _missionFactoryMock;
        private Mock<ILevelService> _levelServiceMock;
        private Mock<IMissionRepository> _missionRepositoryMock;
        private Mock<UserManager<UserEntity>> _userManagerMock;
        private Mock<IMinerShipManager> _minerShipManager;
        
        [SetUp]
        public void SetUp()
        {
            _spaceShipRepositoryMock = new Mock<ISpaceShipRepository>();
            _spaceStationRepositoryMock = new Mock<ISpaceStationRepository>();
            _shipManagerFactoryMock = new Mock<IShipManagerFactory>();
            _missionFactoryMock = new Mock<IMissionFactory>();
            _levelServiceMock = new Mock<ILevelService>();
            _missionRepositoryMock = new Mock<IMissionRepository>();
            _userManagerMock = IdentityMocks.MockUserManager<UserEntity>();
            
            _shipService = new ShipService(
                _userManagerMock.Object,
                _spaceShipRepositoryMock.Object,
                _spaceStationRepositoryMock.Object,
                _shipManagerFactoryMock.Object, 
                _missionFactoryMock.Object,
                _levelServiceMock.Object,
                _missionRepositoryMock.Object
            );
        }

        [Test]
        public async Task GetAllShips_ReturnsAllShips_ForAdminUser()
        {
   
            var adminClaimsPrincipal = IdentityMocks.CreateClaimsPrincipal("Admin");
            var mockShips = new List<SpaceShip> { new MinerShip(), new MinerShip() };
            _spaceShipRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockShips);
            
            var result = await _shipService.GetAllShips(adminClaimsPrincipal);
 
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task CreateShip_CreatesNewShip_ForValidInput()
        {
            var userId = Guid.NewGuid().ToString();
  
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "sanyi"),
                new Claim(ClaimTypes.Email, "sanyi@gmail.com"),
            };
            
            var userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
            var currentUser = new UserEntity { Id = userId, UserName = "sanyi"};
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);
            

            var newShipDto = new NewShipDTO("Test Ship", ShipColor.RED, ShipType.MINER);
            var mockSpaceShipManager = new Mock<ISpaceShipManager>();

            _levelServiceMock.Setup(service => service.GetLevelByTypeAndLevel(It.IsAny<UpgradeableType>(), It.IsAny<int>()))
                .Returns(new Level());
           
            var mockMinerShip = MinerShip.CreateNewMinerShip(_levelServiceMock.Object, newShipDto.name, newShipDto.color);
            mockMinerShip.User = currentUser;    
            var mockMinerShipManager = new Mock<IMinerShipManager>();
            
            _shipManagerFactoryMock.Setup(factory => factory.GetSpaceShipManager(It.IsAny<SpaceShip>()))
                .Returns(mockSpaceShipManager.Object);
            
            mockMinerShipManager.Setup(manager => manager.CreateNewShip(It.IsAny<ILevelService>(), It.IsAny<string>(), It.IsAny<ShipColor>()))
                .Returns(mockMinerShip);
            mockSpaceShipManager.Setup(manager => manager.GetShip()).Returns(mockMinerShip);

            _spaceShipRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<MinerShip>()))
                .ReturnsAsync(mockMinerShip);
            
            
            var createdShip = await _shipService.CreateShip(newShipDto, userClaimsPrincipal);
            
            Assert.IsNotNull(createdShip);
            Assert.AreEqual(newShipDto.name, createdShip.Name);
            Assert.AreEqual(newShipDto.color, createdShip.Color);
        }
        
        [Test]
        public async Task GetUserAsync_ReturnsUser_ForValidClaimsPrincipal()
        {
            // Arrange
            var userId = "test-user-id";
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "test-user"),
                new Claim(ClaimTypes.Email, "test-user@example.com")
            };
            var userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "mock"));

            var expectedUser = new UserEntity { Id = userId };
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(expectedUser);

            // Act
            var actualUser = await _userManagerMock.Object.GetUserAsync(userClaimsPrincipal);

            // Assert
            Assert.IsNotNull(actualUser);
            Assert.AreEqual(expectedUser.Id, actualUser.Id);
        }
        
        [Test]
        public async Task GetShipsByStationAsync_ReturnsShipsForAuthorizedUser()
        {
            // Arrange
            var userId = "test-user-id";
            var stationId = 1;
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "User") // Assuming "User" is the role
            };
            var userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, "mock"));
            var currentUser = new UserEntity { Id = userId };
            _userManagerMock.Setup(manager => manager.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);

            var station = new SpaceStation { User = currentUser };
            _spaceStationRepositoryMock.Setup(repo => repo.GetByIdAsync(stationId)).ReturnsAsync(station);

            var mockShips = new List<SpaceShip> { new MinerShip(), new MinerShip() };
            _spaceShipRepositoryMock.Setup(repo => repo.GetByStationIdAsync(stationId)).ReturnsAsync(mockShips);

            // Act
            var result = await _shipService.GetShipsByStationAsync(stationId, userClaimsPrincipal);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

    }
}
