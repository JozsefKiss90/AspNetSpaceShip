using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SpaceshipAPI;
using SpaceShipAPI.Model.DTO;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceshipAPI.Spaceship.Model.Station;
namespace SpaceShipAPI.Services;
public class SpaceStationService : ISpaceStationService
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly ISpaceStationRepository _spaceStationRepository;
    private readonly ISpaceShipRepository _spaceShipRepository;
    private readonly IShipManagerFactory _shipManagerFactory;
    private readonly ILevelService _levelService;
    private readonly ISpaceStationManager _spaceStationManager;

    public SpaceStationService (
        UserManager<UserEntity> userManager,
        ISpaceStationRepository spaceStationRepository,
        ISpaceShipRepository spaceShipRepository,
        IShipManagerFactory shipManagerFactory,
        ILevelService levelService,
        ISpaceStationManager spaceStationManager
        )
    {
        _userManager = userManager;
        _spaceStationRepository = spaceStationRepository;
        _spaceShipRepository = spaceShipRepository;
        _shipManagerFactory = shipManagerFactory;
        _levelService = levelService;
        _spaceStationManager = spaceStationManager;
    }

    public async Task<SpaceStationDTO> GetBaseByIdAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await GetStationByIdAndCheckAccessAsync(stationId, user);
        if (station.Hangar == null)
        {
            station.Hangar = new HashSet<SpaceShip>();
        }
        //var stationManager = new SpaceStationManager(station, _levelService, new HangarManager(_levelService, 1, new HashSet<SpaceShip>(station.Hangar)));
        return _spaceStationManager.GetStationDTO(station);
    }
    
    public async Task<SpaceStationDTO> CreateAsync(string name, ClaimsPrincipal userPrincipal)
    {
        var currentUser = await GetCurrentUserAsync(userPrincipal);
    
        SpaceStation spaceStation = _spaceStationManager.CreateNewSpaceStation(name);
        spaceStation.User = currentUser;
        spaceStation.StorageLevel = 1;
        spaceStation.HangarLevel = 1;
        spaceStation.StoredResources = new List<StoredResource>
        {
            new StoredResource { ResourceType = ResourceType.PLUTONIUM, Amount = 0 },
            new StoredResource { ResourceType = ResourceType.CRYSTAL, Amount = 20 },
            new StoredResource { ResourceType = ResourceType.SILICONE, Amount = 20 },
            new StoredResource { ResourceType = ResourceType.METAL, Amount = 50 }
        }; 
        var createdStation = await _spaceStationRepository.CreateAsync(spaceStation);
        return ConvertToDTO(createdStation);
    }


    public async Task<bool> AddResourcesAsync(long id, Dictionary<ResourceType, int> resources, ClaimsPrincipal user)
    { 
        var station = await GetStationByIdAndCheckAccessAsync(id, user);
        int value;
        foreach (var resource in resources.Keys)
        {
            _spaceStationManager.AddResource(resource, resources[resource], station);
        }
        await _spaceStationRepository.UpdateAsync(station);
        return true;
    }

    public async Task<SpaceStationDataDTO> GetBaseDataForCurrentUserAsync(ClaimsPrincipal user)
    {
        var userEntity = await GetCurrentUser(user); 
        var station = await _spaceStationRepository.GetByUserAsync(userEntity);
        if (station == null)
        {
            throw new KeyNotFoundException("No station found for user");
        }
        return new SpaceStationDataDTO(station);
    }
    
    public async Task<long> AddShipAsync(long stationId, NewShipDTO newShipDTO, ClaimsPrincipal user)
    {
        var station = await GetStationByIdAndCheckAccessAsync(stationId, user);
        
        SpaceShip ship;
        var userId = _userManager.GetUserId(user);
        if (newShipDTO.type == ShipType.MINER)
        {
            ship = MinerShipManager.CreateNewMinerShip(_levelService, newShipDTO.name, newShipDTO.color);
            _spaceStationManager.AddNewShip(ship, ShipType.MINER, station);
        }
        else if (newShipDTO.type == ShipType.SCOUT)
        {
            ship = ScoutShipManager.CreateNewScoutShip(_levelService, newShipDTO.name, newShipDTO.color);
            _spaceStationManager.AddNewShip(ship, ShipType.SCOUT, station);
        }
        else
        {
            throw new ArgumentException("Ship type not recognized");
        }
        
        ship.UserId = userId;
        
        await _spaceShipRepository.CreateAsync(ship);
        return ship.Id;
    }
    
    public async Task<Dictionary<ResourceType, int>> GetStorageUpgradeCostAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await GetStationByIdAndCheckAccessAsync(stationId, user);

        return _spaceStationManager.GetStorageUpgradeCost(station);
    }

    
    public async Task<SpaceStationStorageDTO> GetStationStorageAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await GetStationByIdAndCheckAccessAsync(stationId, user);

        return _spaceStationManager.GetStorageDTO(station);
    }

    public async Task<HangarDTO> GetStationHangarAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await GetStationByIdAndCheckAccessAsync(stationId, user);
        return _spaceStationManager.GetHangarDTO(station);
    }
    
    /*public async Task<HangarDTO> GetStationHangarAsync(long stationId, ClaimsPrincipal user)
    {
        var hangar = await UpgradeHangarDockAsync(stationId, user);
        return hangar;
    }*/
    
    public async Task<bool> UpgradeStorageAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await GetStationByIdAndCheckAccessAsync(stationId, user);
        if (_spaceStationManager.UpgradeStorage(station))
        {
            await _spaceStationRepository.UpdateAsync(station);
            return true;
        }
        return false;
    }
    
    public async Task<Dictionary<ResourceType, int>> GetHangarUpgradeCostAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await _spaceStationRepository.GetByIdAsync(stationId);

        return _spaceStationManager.GetHangarUpgradeCost(station);
    }
    
    public async Task<bool> UpgradeHangarAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await GetStationByIdAndCheckAccessAsync(stationId, user);
        if (_spaceStationManager.UpgradeHangar(station))
        {
            await _spaceStationRepository.UpdateAsync(station);
            return true;
        }
        return false;
    }

    public async Task<HangarDTO> UpgradeHangarDockAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await GetStationByIdAndCheckAccessAsync(stationId, user);
        var oldHangar = await GetStationHangarAsync(stationId, user);
        var newHangar = _spaceStationManager.UpdateHangarDTO(oldHangar);
        return newHangar;
    }
    
    public async Task<bool> MoveResourceFromShipToStationAsync(long stationId, long shipId, Dictionary<ResourceType, int> resources, ClaimsPrincipal user)
    {
        var station = await GetStationByIdAndCheckAccessAsync(stationId, user);
        var ship = await _spaceShipRepository.GetByIdAsync(shipId);
        if (ship == null || ship.SpaceStationId != station.Id)
        {
            throw new KeyNotFoundException("No such ship on this station");
        }
       
        if (_spaceStationManager.AddResourcesFromShip((MinerShipManager)_spaceStationManager, resources, station))
        {
            await _spaceStationRepository.UpdateAsync(station);
            return true;
        }
        return false;
    }
    
    public async Task<Dictionary<ResourceType, int>> GetStoredResourcesAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await _spaceStationRepository.GetByIdAsync(stationId);
        return _spaceStationManager.GetStoredResources(station);
    }

    public async Task<SpaceStation> GetStationByIdAndCheckAccessAsync(long stationId, ClaimsPrincipal user)
    {
        var station = await _spaceStationRepository.GetByIdAsync(stationId);
        if (station == null)
        {
            throw new KeyNotFoundException($"No station found with id {stationId}");
        }

        var currentUser = await GetCurrentUser(user);
       // var isAdmin = user.IsInRole("Admin"); 
        if (currentUser.Id.ToString() != station.User.Id)
        {
            throw new UnauthorizedAccessException("You don't have authority to access this station");
        }

        return station;
    }
    
    private string GetUserIdFromPrincipal(ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private async Task<UserEntity> GetCurrentUser(ClaimsPrincipal user)
    {
        var userId = GetUserIdFromPrincipal(user);
        return await _userManager.FindByIdAsync(userId);
    }
    
    private async Task<UserEntity> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
    {
        return await _userManager.GetUserAsync(userPrincipal);
    }
    
    public SpaceStationDTO ConvertToDTO(SpaceStation station)
    {
        return new SpaceStationDTO(
            station.Id,
            station.Name,
            _spaceStationManager.GetHangarDTO(station),
            _spaceStationManager.GetStorageDTO(station)
        );
    }
    
    public async Task DeleteSpaceStationAsync(long spaceStationId)
    {
       await _spaceStationRepository.DeleteAsync(spaceStationId);
    }

}
