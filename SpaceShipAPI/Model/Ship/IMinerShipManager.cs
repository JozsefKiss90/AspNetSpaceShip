using SpaceShipAPI.Model.DTO.Ship;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Services;

namespace SpaceShipAPI.Model.Ship
{
    public interface IMinerShipManager
    {
        SpaceShip CreateNewShip(ILevelService levelService, string name, ShipColor color);
        ShipDetailDTO GetDetailedDTO();
        HashSet<ShipPart> GetPartTypes();
        Dictionary<ResourceType, int> GetUpgradeCost(ShipPart part);
        int GetDrillEfficiency();
        int GetEmptyStorageSpace();
        bool AddResourceToStorage(ResourceType resourceType, int amount);
        bool UpgradePart(ShipPart part);
        Dictionary<ResourceType, int> GetCost();
        bool HasResourcesInStorage(Dictionary<ResourceType, int> resources);
    }
}
