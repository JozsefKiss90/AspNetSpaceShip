using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SpaceshipAPI;
using Xunit;
using SpaceShipAPI;
using SpaceShipAPI.Database; // Replace with your actual namespace

namespace Intergration
{
    public class AuthenticationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public AuthenticationIntegrationTests(WebApplicationFactory<Program> factory)
        {
           _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureTestServices(services =>
                {
                    // Mock UserManager and RoleManager  
                    var currentUser = new UserEntity { Id = "test_user_id", UserName = "test_user"};

                    var userStoreMock = new Mock<IUserStore<UserEntity>>();
                    var userManagerMock = new Mock<UserManager<UserEntity>>(
                        userStoreMock.Object, null, null, null, null, null, null, null, null);

                    userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((string email) => new UserEntity { Email = email });
                    userManagerMock.Setup(um => um.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
                    userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

                    var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();

                    var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                        roleStoreMock.Object,
                        new IRoleValidator<IdentityRole>[0], // Empty array of IRoleValidator<IdentityRole>
                        new Mock<ILookupNormalizer>().Object,
                        new Mock<IdentityErrorDescriber>().Object,
                        new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

                    // Setup the mock behavior as needed
                    roleManagerMock.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
                    roleManagerMock.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

                    // Replace UserManager and RoleManager with mocks
                    services.AddScoped<UserManager<UserEntity>>(_ => userManagerMock.Object);
                    services.AddScoped<RoleManager<IdentityRole>>(_ => roleManagerMock.Object);
                    
                    services.Remove(services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)));
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                    
                    // Configure JWT authentication using a test signing key only if it's not already configured
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

        _client = _factory.CreateClient();
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
        public async Task Authentication_ShouldSucceed_WithValidToken()
        {
            // Arrange
            var token = GenerateTestUserToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/Auth/TestAuthentication"); 

            // Assert
            Assert.True(response.IsSuccessStatusCode, "Authentication failed. Response status code: " + response.StatusCode);
        }
    }
}
