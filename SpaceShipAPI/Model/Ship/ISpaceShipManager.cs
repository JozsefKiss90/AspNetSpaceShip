using SpaceShipAPI.Model.DTO.Ship;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;

namespace SpaceShipAPI.Model.Ship
{
    public interface ISpaceShipManager
    {
        SpaceShip GetShip();
        bool IsAvailable();
        Mission.Mission GetCurrentMission();
        void SetCurrentMission(Mission.Mission mission);
        void EndMission();
        int GetShieldEnergy();
        int GetShieldMaxEnergy();
        void RepairShield(int amount);
        void DamageShield(int amount);
        int GetSpeed();
        ShipDetailDTO GetDetailedDTO();
        HashSet<ShipPart> GetPartTypes();
        bool UpgradePart(ShipPart part);
        Dictionary<ResourceType, int> GetCost();
        Dictionary<ResourceType, int> GetUpgradeCost(ShipPart part);
    }
}
