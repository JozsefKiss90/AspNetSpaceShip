using System.Security.Claims;
using SpaceShipAPI.Model.DTO.Ship;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;

namespace SpaceShipAPI.Services
{
    public interface IShipService
    {
        Task<SpaceShip> GetByIdAsync(long id);
        Task<IEnumerable<ShipDTO>> GetAllShips(ClaimsPrincipal user);
        Task<IEnumerable<ShipDTO>> GetShipsByStationAsync(long stationId, ClaimsPrincipal user);
        Task<ShipDTO> GetShipDetailsByIdAsync(long id);
        Task<ShipDTO> CreateShip(NewShipDTO newShip, ClaimsPrincipal userPrincipal);
        Task<SpaceShip> UpdateAsync(SpaceShip ship);
        Task<ShipDetailDTO> UpgradeShipPartAsync(long id, ShipPart part);
        Task<bool> DeleteShipByIdAsync(long id);
        Task UpdateMissionIfExists(SpaceShip ship);
        ShipColor[] getColors();
        Task DeleteAsync(long id);
        Task<Dictionary<ResourceType, int>> GetShipCost(ShipType shipType);
    }
}