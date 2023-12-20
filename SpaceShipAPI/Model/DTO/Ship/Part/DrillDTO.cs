using SpaceShipAPI.Model.Ship.ShipParts;

namespace SpaceShipAPI.Model.DTO.Ship.Part;

public record DrillDTO(int Level, int Efficiency, bool FullyUpgraded)
{
    public DrillDTO(DrillManager drillManager) 
        : this(drillManager.GetCurrentLevel(), drillManager.Efficiency, drillManager.IsFullyUpgraded())
    {
    }
}
