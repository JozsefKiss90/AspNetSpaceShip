using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SpaceShipAPI.Model.DTO;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceshipAPI.Model;
using Microsoft.AspNetCore.Identity;
using SpaceshipAPI;
using SpaceshipAPI.Spaceship.Model.Station;

namespace SpaceShipAPI.Services
{
    public interface ISpaceStationService
    {
        Task<SpaceStationDTO> GetBaseByIdAsync(long stationId, ClaimsPrincipal user);

        Task<SpaceStationDTO> CreateAsync(string name, ClaimsPrincipal user);

        Task<bool> AddResourcesAsync(long id, Dictionary<ResourceType, int> resources, ClaimsPrincipal user);

        Task<SpaceStationDataDTO> GetBaseDataForCurrentUserAsync(ClaimsPrincipal user);

        Task<long> AddShipAsync(long stationId, NewShipDTO newShipDTO, ClaimsPrincipal user);

        Task<Dictionary<ResourceType, int>> GetStorageUpgradeCostAsync(long stationId, ClaimsPrincipal user);

        Task<SpaceStationStorageDTO> GetStationStorageAsync(long stationId, ClaimsPrincipal user);

        Task<HangarDTO> GetStationHangarAsync(long stationId, ClaimsPrincipal user);

        Task<bool> UpgradeStorageAsync(long stationId, ClaimsPrincipal user);

        Task<Dictionary<ResourceType, int>> GetHangarUpgradeCostAsync(long stationId, ClaimsPrincipal user);

        Task<bool> UpgradeHangarAsync(long stationId, ClaimsPrincipal user);

        Task<bool> MoveResourceFromShipToStationAsync(long stationId, long shipId, Dictionary<ResourceType, int> resources, ClaimsPrincipal user);

        Task<Dictionary<ResourceType, int>> GetStoredResourcesAsync(long stationId, ClaimsPrincipal user);

        Task<SpaceStation> GetStationByIdAndCheckAccessAsync(long stationId, ClaimsPrincipal user);
        Task<HangarDTO> UpgradeHangarDockAsync(long stationId, ClaimsPrincipal user);
        Task DeleteSpaceStationAsync(long spaceStationId);
    }
}