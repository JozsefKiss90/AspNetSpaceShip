using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SpaceshipAPI.Model.Ship;
using Xunit;
using Assert = NUnit.Framework.Assert;

public class SpaceShipControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public SpaceShipControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Additional configuration for the test server can be specified here
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsShips_WhenUserAuthenticated()
    {
        // Arrange - set up any required authentication, headers, etc.
        var token = GetTestUserToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/spaceship");
        response.EnsureSuccessStatusCode();
        var ships = await response.Content.ReadFromJsonAsync<List<SpaceShip>>();

        // Assert
        Assert.NotNull(ships);
    }

    private string GetTestUserToken()
    {
        // Mock the token generation process or use a pre-generated valid token
        return "mocked_token";
    }
}