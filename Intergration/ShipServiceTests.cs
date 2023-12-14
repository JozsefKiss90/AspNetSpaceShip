using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SpaceShipAPI.Database;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceShipAPI.Services;
using Xunit;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using SpaceshipAPI;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;

namespace IntegrationTests
{
    public class ShipServiceTests
    {
        private readonly IShipService _shipService;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<UserEntity> _userManager;
        private readonly Mock<ISpaceShipRepository> _spaceShipRepoMock;
        
        public ShipServiceTests()
        {
            var services = new ServiceCollection();

            // Mock dependencies
            var userStoreMock = new Mock<IUserStore<UserEntity>>();
            var userManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Create UserManager with mocked user store
            var userManager = new UserManager<UserEntity>(
                userStoreMock.Object, 
                null, null, null, null, null, null, null, null);            var spaceShipRepoMock = new Mock<ISpaceShipRepository>();
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UserEntity { Id = "test_user_id", UserName = "test_user", Email = "test@example.com" });
            
            var spaceStationRepoMock = new Mock<ISpaceStationRepository>();
            var shipManagerFactoryMock = new Mock<IShipManagerFactory>();
            var missionFactoryMock = new Mock<IMissionFactory>();
            var levelServiceMock = new Mock<ILevelService>();
            var missionRepositoryMock = new Mock<IMissionRepository>();

            // Set up in-memory database
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));
    
            // Add mocks to the service collection
            services.AddSingleton<UserManager<UserEntity>>(userManagerMock.Object);
            services.AddSingleton(spaceShipRepoMock.Object);
            services.AddSingleton(spaceStationRepoMock.Object);
            services.AddSingleton(shipManagerFactoryMock.Object);
            services.AddSingleton(missionFactoryMock.Object);
            services.AddSingleton(levelServiceMock.Object);
            services.AddSingleton(missionRepositoryMock.Object);

            // Add ShipService with the mocked dependencies
            services.AddScoped<IShipService, ShipService>();
                
            var serviceProvider = services.BuildServiceProvider();
            _dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            _shipService = serviceProvider.GetRequiredService<IShipService>();

            // Seed the database
            _dbContext.Users.Add(new UserEntity { Id = "test_user_id", UserName = "test_user" });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task CreateShip_ShouldCreateShip_WhenUserExists()
        {
            // Arrange
            var newShipDTO = new NewShipDTO("Test Ship", ShipColor.RED, ShipType.MINER);
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test_user_id")
            }));

            // Act
            var result = await _shipService.CreateShip(newShipDTO, userPrincipal);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ShipDTO>(result);
        }

        // Additional tests can be written to test different scenarios
    }
}
