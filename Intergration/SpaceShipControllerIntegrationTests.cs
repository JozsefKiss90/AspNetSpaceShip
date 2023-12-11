using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.IdentityModel.Tokens;
using SpaceshipAPI;
using SpaceShipAPI.Model.DTO.Ship;
using Moq;
using SpaceShipAPI;
using SpaceShipAPI.Database;
using SpaceShipAPI.Model.DTO.Ship.Part;
using SpaceShipAPI.Model.Mission;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Services;

namespace Intergration;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using SpaceshipAPI.Model.Ship;
using Xunit;

public class SpaceShipControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _testUserToken;

    public SpaceShipControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
       _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureTestServices(services =>
                {
                    var serviceProvider = services.BuildServiceProvider();

                    services.Remove(services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)));
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                    
                    using (var scope = services.BuildServiceProvider().CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        dbContext.Database.EnsureCreated();

                        // Add test user
                        var newTestUser = new UserEntity { Id = "test_user_id", UserName = "test_user", Email = "test@example.com" };
                        dbContext.Users.Add(newTestUser);
                        dbContext.SaveChanges();
                    }
                    
                    var currentUser = new UserEntity { Id = "test_user_id", UserName = "test_user"};

                    var userStoreMock = new Mock<IUserStore<UserEntity>>();
                    var userManagerMock = new Mock<UserManager<UserEntity>>(
                        userStoreMock.Object, null, null, null, null, null, null, null, null);
                    var testUser = new UserEntity { Id = "test_user_id", UserName = "test_user", Email = "test@example.com" };
                
                    userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(testUser);
                    userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((string email) => new UserEntity { Email = email });
                    userManagerMock.Setup(um => um.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
                    userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

                    var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();

                    var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                        roleStoreMock.Object,
                        new IRoleValidator<IdentityRole>[0], 
                        new Mock<ILookupNormalizer>().Object,
                        new Mock<IdentityErrorDescriber>().Object,
                        new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

                    roleManagerMock.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
                    roleManagerMock.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
                    
                    long id = 1;
                    string name = "TestShip";
                    ShipColor color = ShipColor.RED;
                    ShipType type = ShipType.MINER;
                    
                    EngineDTO engine = new EngineDTO(1, 100, true); 
                    ShieldDTO shield = new ShieldDTO(1, 50, 100, false); 
                    DrillDTO drill = new DrillDTO(1, 1, false);
                    ShipStorageDTO storage = new ShipStorageDTO(1, 5, new Dictionary<ResourceType, int>
                    {
                        { ResourceType.METAL, 2 }
                    }, false);
                    ShipDetailDTO shipDetailDTO = new MinerShipDTO(id, name, color, type, new MiningMission(), engine, shield, drill, storage);
                    var shipServiceMock = new Mock<IShipService>();
                 
                    shipServiceMock.Setup(service => service.GetShipDetailsByIdAsync(It.IsAny<long>()))
                        .ReturnsAsync(shipDetailDTO);
                    
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, "test_user_id"),
                        new Claim(ClaimTypes.Name, "test_user"),
                        new Claim(ClaimTypes.Email, "test@example.com")
                    };
                    var userClaimsIdentity = new ClaimsIdentity(userClaims, "TestAuthentication");
                    var userClaimsPrincipal = new ClaimsPrincipal(userClaimsIdentity);

                    shipServiceMock.Setup(service => service.CreateShip(It.IsAny<NewShipDTO>(), It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync((NewShipDTO newShipDTO, ClaimsPrincipal principal) =>
                        {
                            using (var scope = serviceProvider.CreateScope())
                            {
                                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                                var userExists = dbContext.Users.Any(u => u.Id == userId);
                                if (!userExists)
                                {
                                    throw new InvalidOperationException("User not found");
                                }

                                var shipDto = new ShipDTO(1, newShipDTO.name, newShipDTO.color, newShipDTO.type, 0);
                                return shipDto;
                            }
                        });

                    services.AddScoped<UserManager<UserEntity>>(_ => userManagerMock.Object);
                    services.AddScoped<RoleManager<IdentityRole>>(_ => roleManagerMock.Object);
                    
                    services.AddScoped<DbContextInitializer>();
                    services.AddScoped<IShipService>(_ => shipServiceMock.Object);
                    
                    if (!services.Any(x => x.ServiceType == typeof(IAuthenticationHandlerProvider)))
                    {
                        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("!SomethingSecret!"));
                        services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        }).AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = signingKey,
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ClockSkew = TimeSpan.Zero,
                                ValidIssuer = "apiWithAuthBackend",
                                ValidAudience = "apiWithAuthBackend",
                            };
                        });
                    }
                });
            });
        
       using (var scope = _factory.Services.CreateScope())
       {
           var initializer = scope.ServiceProvider.GetRequiredService<DbContextInitializer>();
       }

        _client = _factory.CreateClient();
        // Generate JWT token for the test user
        _testUserToken = GenerateTestUserToken();
    }

    private string GenerateTestUserToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("!SomethingSecret!"); 
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test_user_id"),
                new Claim(ClaimTypes.Name, "test_user"),
                new Claim(ClaimTypes.Role, "User"), 
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = "apiWithAuthBackend",
            Audience = "apiWithAuthBackend",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("!SomethingSecret!")), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsShips_WhenUserAuthenticated()
    {
        // Arrange - set up any required authentication, headers, etc.
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testUserToken); //debuggers returns valid token for _testUserToken 

        // Act
        var response = await _client.GetAsync("/api/spaceships"); 
        response.EnsureSuccessStatusCode();
        var shipsDto = await response.Content.ReadFromJsonAsync<List<ShipDTO>>();
        
        // Assert
        Assert.NotNull(shipsDto);
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsShipDetails()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testUserToken);
        long shipId = 1; // Example ship ID

        // Act
        var response = await _client.GetAsync($"/api/spaceships/{shipId}");
        response.EnsureSuccessStatusCode();

        var shipDetails = await response.Content.ReadFromJsonAsync<ShipDTO>();

        // Assert
        Assert.NotNull(shipDetails);
    }

    [Fact]
    public async Task CreateAsync_CreatesShip()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testUserToken);
        NewShipDTO newShip = new NewShipDTO("Test Ship", ShipColor.RED, ShipType.MINER);

        var response = await _client.PostAsJsonAsync("/api/spaceships", newShip);
        response.EnsureSuccessStatusCode();

        var createdShip = await response.Content.ReadFromJsonAsync<ShipDTO>();
        Assert.NotNull(createdShip);
        Assert.Equal("Test Ship", createdShip.Name);
        Assert.Equal(ShipColor.RED, createdShip.Color);
        Assert.Equal(ShipType.MINER, createdShip.Type);
    }
    
}

public class DbContextInitializer
{
    private readonly AppDbContext _context;

    public DbContextInitializer(AppDbContext context)
    {
        _context = context;
        Initialize();
    }

    private void Initialize()
    {
        // Delete the existing database and create a new one
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        // Add test user
        var testUser = new UserEntity { Id = "test_user_id", UserName = "test_user", Email = "test@example.com" };
        if (!_context.Users.Any(u => u.Id == testUser.Id))
        {
            _context.Users.Add(testUser);
            _context.SaveChanges();
        }
    }
}
