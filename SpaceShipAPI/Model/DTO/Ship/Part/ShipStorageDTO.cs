using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Utils;

namespace SpaceShipAPI.Model.DTO.Ship.Part;

using System.Collections.Generic;

public record ShipStorageDTO(int Level, int MaxCapacity, Dictionary<ResourceType, int> Resources, bool FullyUpgraded)
{
    public ShipStorageDTO(ShipStorageManager shipStorageManager) 
        : this(shipStorageManager.GetCurrentLevel(), shipStorageManager.GetCurrentCapacity(), ResourceUtility.ConvertToDictionary(shipStorageManager.GetStoredResources()), shipStorageManager.IsFullyUpgraded())
    {
    }
}
