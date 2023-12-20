using Microsoft.EntityFrameworkCore;
using SpaceshipAPI;
using SpaceShipAPI.Database; 
using Xunit;

public class AppDbContextTests
{
    private readonly AppDbContext _dbContext;

    public AppDbContextTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _dbContext = new AppDbContext(options);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _dbContext.Users.Add(new UserEntity { Id = "test_user_id", UserName = "test_user", Email = "test@example.com" });
        _dbContext.SaveChanges();
    }

    [Fact]
    public void TestDbContextConnection()
    {
        // Implement your test logic here
        var user = _dbContext.Users.Find("test_user_id");
        Assert.NotNull(user);
    }

}