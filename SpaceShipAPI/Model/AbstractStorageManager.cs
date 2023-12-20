using SpaceShipAPI.Model.Exceptions;
using SpaceShipAPI.Repository;
using SpaceShipAPI.Services;

namespace SpaceShipAPI.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using SpaceShipAPI.Model; 

public abstract class AbstractStorageManager : Upgradeable
{
    protected ICollection<StoredResource> StoredResources { get; }

    protected AbstractStorageManager(ILevelService levelService, UpgradeableType type, int level, ICollection<StoredResource> storedResources)
        : base(levelService, type, level)
    {
        int totalResources = storedResources.Sum(sr => sr.Amount);
        if (totalResources > CurrentLevel.Effect + 1)
        {
            throw new ResourceCapacityExceededException($"Stored resources can't exceed {CurrentLevel.Effect} at this level");
        }
        StoredResources = storedResources;
    }

    public int GetCurrentCapacity()
    {
        return CurrentLevel.Effect;
    }

    public int GetCurrentAvailableStorageSpace()
    {
        return GetCurrentCapacity() - StoredResources.Sum(sr => sr.Amount);
    }

    public ICollection<StoredResource> GetStoredResources()
    {
        return StoredResources;
    }

    public bool AddResource(ResourceType resourceType, int quantity)
    {
        if (quantity < 0)
        {
            throw new NegativeResourceAdditionException();
        }

        int availableSpace = GetCurrentAvailableStorageSpace();
        if (quantity > availableSpace)
        {
            throw new InsufficientStorageSpaceException();
        }
        
        var existingResource = StoredResources.FirstOrDefault(sr => sr.ResourceType == resourceType);

        if (existingResource != null)
        {
            existingResource.Amount += quantity;
        }
        else
        {
            StoredResources.Add(new StoredResource { ResourceType = resourceType, Amount = quantity });
        }

        return true;
    }


    public bool HasResource(ResourceType resourceType, int quantity)
    {
        var existingResource = StoredResources.FirstOrDefault(sr => sr.ResourceType == resourceType);

        if (existingResource != null)
        {
            return existingResource.Amount >= quantity;
        }

        return false; // Resource not found
    }


    public bool RemoveResource(ResourceType resourceType, int quantity)
    {
        if (quantity < 0)
        {
            throw new Exception("Can't remove negative resources.");
        }

        var existingResource = StoredResources.FirstOrDefault(sr => sr.ResourceType == resourceType);

        if (existingResource != null)
        {
            if (existingResource.Amount >= quantity)
            {
                existingResource.Amount -= quantity;
                return true;
            }
            else
            {
                throw new Exception("Not enough resource.");
            }
        }

        throw new Exception("Resource not found.");
    }

}
