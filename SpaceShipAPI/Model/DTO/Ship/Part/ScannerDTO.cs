using SpaceShipAPI.Model.Ship.ShipParts;

namespace SpaceShipAPI.Model.DTO.Ship.Part;

public record ScannerDTO(int Level, int Efficiency, bool FullyUpgraded)
{
    public ScannerDTO(ScannerManager scannerManager) 
        : this(scannerManager.GetCurrentLevel(), scannerManager.Efficiency, scannerManager.IsFullyUpgraded())
    {
    }
}
