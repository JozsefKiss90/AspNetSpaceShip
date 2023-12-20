using SpaceShipAPI.Services;
using SpaceShipAPI.Utils;

namespace SpaceShipAPI.Model.Ship.ShipParts;

public class ShipStorageManager : AbstractStorageManager
{
    private static readonly UpgradeableType TYPE = UpgradeableType.SHIP_STORAGE; 

    public ShipStorageManager(ILevelService levelService, int currentLevel, IDictionary<ResourceType, int> storedResources) 
        : base(levelService, TYPE, currentLevel, ResourceUtility.MapToStoredResources(storedResources))
    {
    }

    public ShipStorageManager(ILevelService levelService) 
        : this(levelService, 1, new Dictionary<ResourceType, int>()) 
    {
    }
    
}
