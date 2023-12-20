using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpaceshipAPI;
using SpaceShipAPI.Model.Location;
using SpaceShipAPI.Model.Mission;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceshipAPI.Spaceship.Model.Station;
using SpaceShipAPI.Utils;

namespace SpaceShipAPI.Database;

public class AppDbContext : IdentityDbContext<UserEntity>
{
    public DbSet<MinerShip> MinerShips { get; set; }
    public DbSet<ScoutShip> ScoutShips { get; set; }
    
    public DbSet<SpaceStation> SpaceStation { get; set; }
    public DbSet<StoredResource> StoredResources { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<Mission> Missions { get; set; }
    public DbSet<Location> Locations { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>().ToTable("AspNetUsers");

        builder.Entity<SpaceShip>()
            .HasOne<UserEntity>(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<SpaceStation>()
            .HasMany(s => s.StoredResources)
            .WithOne(sr => sr.SpaceStation)
            .HasForeignKey(sr => sr.SpaceStationId)
            .IsRequired(false); 
        
        builder
            .Entity<StoredResource>()
            .Property(e => e.ResourceType)
            .HasConversion<string>(); 

        builder.Entity<SpaceShip>()
            .HasOne<UserEntity>(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<Mission>()
            .HasDiscriminator<string>("MissionType")
            .HasValue<MiningMission>("Mining")
            .HasValue<ScoutingMission>("Scouting");
        
       // LevelSeedData.Seed(builder);
    }
}