using SpaceShipAPI.Model.Ship.ShipParts;

namespace SpaceShipAPI.Model.DTO.Ship.Part;

public record ShieldDTO(int level, int energy, int maxEnergy, bool fullyUpgraded){

    public ShieldDTO(ShieldManager shieldManager) 
        :this(shieldManager.GetCurrentEnergy(), shieldManager.GetCurrentEnergy(), shieldManager.GetMaxEnergy(), shieldManager.IsFullyUpgraded())

    {
        
    }
}