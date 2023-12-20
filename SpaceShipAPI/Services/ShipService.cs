using Microsoft.AspNetCore.Identity;
using SpaceshipAPI;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceShipAPI.Model.Mission;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;

namespace SpaceShipAPI.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class ShipService : IShipService
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly ISpaceShipRepository _spaceShipRepository;
    private readonly ISpaceStationRepository _spaceStationRepository;
    private readonly IShipManagerFactory _shipManagerFactory;
    private readonly IMissionFactory _missionFactory;
    private readonly ILevelService _levelService;
    private readonly IMissionRepository _missionRepository;
    
    public ShipService(
        UserManager<UserEntity> userManager,
        ISpaceShipRepository spaceShipRepository,
        ISpaceStationRepository spaceStationRepository,
        IShipManagerFactory shipManagerFactory,
        IMissionFactory missionFactory,
        ILevelService levelService,
        IMissionRepository missionRepository
        )
    {
        _userManager = userManager;
        _spaceShipRepository = spaceShipRepository;
        _shipManagerFactory = shipManagerFactory;
        _missionFactory = missionFactory;
        _levelService = levelService;
        _missionRepository = missionRepository;
        _spaceStationRepository = spaceStationRepository;
    }

    public async Task<SpaceShip> GetByIdAsync(long id)
    {
        var ship = await _spaceShipRepository.GetByIdAsync(id);
        return ship;
    }
    
    public async Task<IEnumerable<ShipDTO>> GetAllShips(ClaimsPrincipal user)
    {
        var currentUser = await GetCurrentUserAsync(user);

        if (user.IsInRole("Admin"))
        {
            var ships = await _spaceShipRepository.GetAllAsync();
            return ships.Select(ship => new ShipDTO(ship));
        }
        else
        {
            var ships = await _spaceShipRepository.GetAllAsync();
            return ships.Select(ship => new ShipDTO(ship));
        } 
    }

    public async Task<IEnumerable<ShipDTO>> GetShipsByStationAsync(long stationId, ClaimsPrincipal user)
    {
        var currentUser = await GetCurrentUserAsync(user);

        var station = await _spaceStationRepository.GetByIdAsync(stationId);
        if (station == null)
        {
            throw new KeyNotFoundException($"No station found with id {stationId}");
        }

        if (user.IsInRole("Admin") || station.User.Id == currentUser.Id) //line 81
        {
            var ships = await _spaceShipRepository.GetByStationIdAsync(stationId);
            return ships.Select(ship => new ShipDTO(ship));
        }
        else
        {
            throw new UnauthorizedAccessException("You do not have access to these ships");
        }
    }

    public async Task<ShipDTO> GetShipDetailsByIdAsync(long id)
    {
        var ship = await _spaceShipRepository.GetByIdAsync(id);
        if (ship == null)
        {
            throw new KeyNotFoundException($"No ship found with id {id}");
        }

        var spaceShipManager = _shipManagerFactory.GetSpaceShipManager(ship);
        await UpdateMissionIfExists(ship);
        return spaceShipManager.GetDetailedDTO();
    }

    public async Task<ShipDTO> CreateShip(NewShipDTO newShip, ClaimsPrincipal userPrincipal)
    {
        var currentUser = await _userManager.GetUserAsync(userPrincipal);
        if (currentUser == null)
        {
            throw new InvalidOperationException("User not found"); 
        } 

        ISpaceShipManager spaceShipManager;
        if (newShip.type == ShipType.MINER)
        {
            var minerShip = MinerShip.CreateNewMinerShip(_levelService, newShip.name, newShip.color); 
            spaceShipManager = _shipManagerFactory.GetSpaceShipManager(minerShip);
        } 
        else if (newShip.type == ShipType.SCOUT)
        { 
            var scoutShip = ScoutShip.CreateNewScoutShip(_levelService, newShip.name, newShip.color);
            spaceShipManager = _shipManagerFactory.GetSpaceShipManager(scoutShip);
        } 
        else 
        {
            throw new Exception("Ship type not recognized");
        }

        var spaceShip = spaceShipManager.GetShip();
        spaceShip.User = currentUser;  
        spaceShip.UserId = currentUser.Id;
        var createdShip = await _spaceShipRepository.CreateAsync(spaceShip); 
        var shipDto = new ShipDTO(createdShip);
        return shipDto;
    }

    
    public async Task<SpaceShip> UpdateAsync(SpaceShip ship)
    {
        SpaceShip updatedShip = await _spaceShipRepository.UpdateAsync(ship);
        return updatedShip;
    }


    public async Task<ShipDetailDTO> UpgradeShipPartAsync(long id, ShipPart part)
    {
        var ship = await _spaceShipRepository.GetByIdAsync(id);
        if (ship == null)
        {
            throw new KeyNotFoundException($"No ship found with id {id}");
        }

        var spaceShipManager = _shipManagerFactory.GetSpaceShipManager(ship);
        var cost = spaceShipManager.GetUpgradeCost(part);
        // stationManager.RemoveResources(cost);
        spaceShipManager.UpgradePart(part);
        await _spaceShipRepository.UpdateAsync(ship);

        return spaceShipManager.GetDetailedDTO();
    }

    public async Task<bool> DeleteShipByIdAsync(long id)
    {
        var ship = await _spaceShipRepository.GetByIdAsync(id);
        if (ship == null)
        {
            throw new KeyNotFoundException($"No ship found with id {id}");
        }

        if (ship.CurrentMission != null)
        {
            throw new InvalidOperationException("Ship can't be deleted while on mission");
        }

        await _spaceShipRepository.DeleteAsync(id);
        return true;
    }

    public async Task UpdateMissionIfExists(SpaceShip ship)
    {
        ISpaceShipManager shipManager = _shipManagerFactory.GetSpaceShipManager(ship);

        var currentMission = shipManager.GetCurrentMission();
        if (currentMission != null)
        {
            var missionManager = _missionFactory.GetMissionManager(currentMission);
            missionManager.SetShipManager(shipManager);
            if (missionManager.UpdateStatus())
            {
                await _missionRepository.UpdateAsync(currentMission);
            }
        }
    }
    
    public ShipColor[] getColors() {
        return Enum.GetValues<ShipColor>();
    }
    
    private async Task<SpaceShip> GetShipByIdAndCheckAccess(long id, ClaimsPrincipal user)
    {
        var ship = await _spaceShipRepository.GetByIdAsync(id);
        if (ship == null)
        {
            throw new KeyNotFoundException($"No ship found with id {id}");
        }

        var currentUser = await GetCurrentUserAsync(user);
        if (currentUser == null)
        {
            throw new UnauthorizedAccessException("You don't have authority to access this ship");
        }

        var isAdmin = user.IsInRole("Admin"); 
        if (!isAdmin && currentUser.Id != ship.UserId)
        {
            throw new UnauthorizedAccessException("You don't have authority to access this ship");
        }

        return ship;
    }
    
    public async Task DeleteAsync(long id)
    {
        _spaceShipRepository.DeleteAsync(id);
    }
    
    private async Task<UserEntity> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
    {
        return await _userManager.GetUserAsync(userPrincipal);
    }
    
    public async Task<Dictionary<ResourceType, int>> GetShipCost(ShipType shipType) {
        if (shipType != null)
        {
            return await _spaceShipRepository.GetShipCost(shipType);
        } else {
            return null;
        }
    }
}
