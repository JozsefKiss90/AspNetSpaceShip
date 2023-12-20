using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpaceshipAPI;
using SpaceShipAPI.Database;
using SpaceShipAPI.Model.Mission;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Repository;
using SpaceShipAPI.Services;
using SpaceShipAPI.Services.Authentication;
using SpaceshipAPI.Spaceship.Model.Station;
using SpaceShipAPI.Utils;

var builder = WebApplication.CreateBuilder(args);

var logger = builder.Services.BuildServiceProvider().GetService<ILogger<Program>>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

ConfigureServices(builder.Services);
var app = builder.Build();
using var scope = app.Services.CreateScope();
if (!builder.Environment.IsEnvironment("Testing"))
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    LevelSeedData.Seed(dbContext);
}

ConfigurePipeline(app);
AddRoles();
AddAdmin();

app.Run();

void ConfigureServices(IServiceCollection services)
{
    /*services.AddControllers(options => 
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);*/
    services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.IgnoreNullValues = true; 
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; 
        });
    
    services.AddEndpointsApiExplorer();
    services.AddHttpClient();
    
    ConfigureSwagger(services);
    ConfigureIdentity(services);
    ConfigureAuthentication(services);
    services.AddScoped<ISpaceShipRepository, SpaceShipRepository>();
    services.AddScoped<ISpaceStationRepository, SpaceStationRepository>();
    services.AddScoped<ILevelRepository, LevelRepository>();
    services.AddScoped<ILocationRepository, LocationRepository>();
    services.AddScoped<IMissionRepository, MissionRepository>();
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IShipService, ShipService>();
    services.AddScoped<ISpaceStationService, SpaceStationService>();
    services.AddScoped<ILevelService, LevelService>();
    services.AddScoped<IMissionFactory, MissionFactory>();
    services.AddScoped<IShipManagerFactory, ShipManagerFactory>();
    services.AddScoped<ISpaceStationManager, SpaceStationManager>();
    services.AddScoped<IHangarManager, HangarManager>();

   /* services.AddDbCo  ntext<DBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    services.AddDbContext<UserContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));*/

   if (!builder.Environment.IsEnvironment("Testing")) 
   {
       services.AddDbContext<AppDbContext>(options =>
           options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
               .LogTo(Console.WriteLine, LogLevel.Information));
   }
    //// "DefaultConnection": "Host=localhost;Database=spaceship;Username=postgres;Password=postgres;Include Error Detail=true;"
    ///     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=spaceship;User Id=sa;Password=myStrong(!)Password;TrustServerCertificate=True;"
  
    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
}

void ConfigurePipeline(WebApplication app) 
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
        // Temporarily disable HTTPS redirection in development environment
        // app.UseHttpsRedirection();
    }
    else
    {
        app.UseHttpsRedirection();
    }
    app.UseCors("CorsPolicy");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
 
}

void ConfigureSwagger(IServiceCollection services)
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
}

void ConfigureIdentity(IServiceCollection services)
{
    builder.Services
        .AddIdentityCore<UserEntity>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>() 
        .AddEntityFrameworkStores<AppDbContext>();
}

void ConfigureAuthentication(IServiceCollection services)
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "apiWithAuthBackend",
                ValidAudience = "apiWithAuthBackend",
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("!SomethingSecret!")
                ),
            };
        });
}

void AddRoles()
{
    using var scope = app.Services.CreateScope(); 
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var tAdmin = CreateAdminRole(roleManager);
    tAdmin.Wait();

    var tUser = CreateUserRole(roleManager);
    tUser.Wait();
}

async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
{
    await roleManager.CreateAsync(new IdentityRole("Admin")); 
}

async Task CreateUserRole(RoleManager<IdentityRole> roleManager)
{
    await roleManager.CreateAsync(new IdentityRole("User"));
}

void AddAdmin()
{
    if (app.Environment.IsEnvironment("Testing"))
    {
        return;
    }
    var tAdmin = CreateAdminIfNotExists();
    tAdmin.Wait();
}

async Task CreateAdminIfNotExists()
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
    var adminInDb = await userManager.FindByEmailAsync("admin@admin.com");
    if (adminInDb == null)
    {
        var admin = new UserEntity { UserName = "admin", Email = "admin@admin.com" };
        var adminCreated = await userManager.CreateAsync(admin, "admin123");

        if (adminCreated.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
public partial class Program { }