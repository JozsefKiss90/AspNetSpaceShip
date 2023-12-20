using System.Collections.Generic;
using System.Linq;
using SpaceShipAPI;
using SpaceshipAPI.Spaceship.Model.Station;
using SpaceShipAPI.Utils;

public record SpaceStationStorageDTO(Dictionary<ResourceType, int> Resources, int Level, int Capacity, int FreeSpace, bool FullyUpgraded);

public class SpaceStationStorageDTOFactory
{
    public static SpaceStationStorageDTO Create(StationStorageManager storageManager)
    {
        var resources = storageManager.GetStoredResources();
        var level = storageManager.GetCurrentLevel();
        
        var capacity = 100;
        var freeSpace = 100;

        var fullyUpgraded = storageManager.IsFullyUpgraded();

        return new SpaceStationStorageDTO(ResourceUtility.ConvertToDictionary(resources), level, capacity, freeSpace, fullyUpgraded);
    }
}

