using SpaceShipAPI;
using SpaceShipAPI.Model.DTO;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Services;
using SpaceShipAPI.Utils;

namespace SpaceshipAPI.Spaceship.Model.Station;

using SpaceShipAPI.Model.Ship;
using System;
using System.Collections.Generic;

public class SpaceStationManager : ISpaceStationManager
{
    private readonly ILevelService levelService;
    private IHangarManager hangar { get; set; }
    private StationStorageManager storage;

    public SpaceStationManager(
   
        ILevelService levelService,
        IHangarManager _hangar 
        )
    {

        this.levelService = levelService;
        this.hangar = _hangar;
    }

    public SpaceStation CreateNewSpaceStation(string name)
    {
        var station = new SpaceStation
        {
            Name = name,
            HangarLevel = 1,
            Hangar = new HashSet<SpaceShip>(),
            StorageLevel = 1,
            StoredResources = new List<StoredResource>()
        };

        foreach (var resourceType in Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>())
        {
            station.StoredResources.Add(new StoredResource
            {
                ResourceType = resourceType,
                Amount = 0
            });
        }
        return station;
    }


    private bool HasEnoughResource(Dictionary<ResourceType, int> cost, SpaceStation station)
    {
        CreateStorageIfNotExists(station);
        foreach (var entry in cost)
        {
            if (!storage.HasResource(entry.Key, entry.Value))
                return false;
        }
        return true;
    }

    public bool RemoveResources(Dictionary<ResourceType, int> cost, SpaceStation station)
    {
        CreateStorageIfNotExists(station);
        if (HasEnoughResource(cost, station))
        {
            foreach (var resource in cost.Keys)
            {
                storage.RemoveResource(resource, cost[resource]);
            }
            return true;
        }
        throw new Exception("Not enough resource");
    }

    public bool AddNewShip(SpaceShip ship, ShipType shipType, SpaceStation station)
    {
        var cost = shipType.GetCost();
        CreateHangarIfNotExists( station);
        if (!HasEnoughResource(cost, station))
        {
            throw new Exception("Not enough resource");
        }
        else if (hangar.GetCurrentAvailableDocks() == 0)
        {
            throw new Exception("No more docks available");
        }
        return hangar.AddShip(ship) && RemoveResources(cost, station);
    }

    public Dictionary<ResourceType, int> GetStoredResources(SpaceStation station)
    {
        CreateStorageIfNotExists(station);
    
        IDictionary<ResourceType, int> storedResources = ResourceUtility.ConvertToDictionary(storage.GetStoredResources());
        
        Dictionary<ResourceType, int> convertedResources = storedResources.ToDictionary(entry => entry.Key, entry => entry.Value);

        return convertedResources;
    }


    public Dictionary<ResourceType, int> GetStorageUpgradeCost(SpaceStation station)
    {
        CreateStorageIfNotExists(station);
        return storage.GetUpgradeCost();
    }

    public  Dictionary<ResourceType, int> GetHangarUpgradeCost(SpaceStation station) {
        CreateHangarIfNotExists(station);
        return hangar.GetUpgradeCost();
    }
    public bool RemoveShip(SpaceShip ship, SpaceStation station)
    {
        CreateHangarIfNotExists(station);
        return hangar.RemoveShip(ship);
    }

    public HashSet<SpaceShip> GetAllShips(SpaceStation station)
    {
        return new HashSet<SpaceShip>(station.Hangar);
    }

    public bool HasShipAvailable(SpaceShip ship, SpaceStation station)
    {
        CreateHangarIfNotExists(station);
        return hangar.HasShipAvailable(ship);
    }
    
    public bool UpgradeHangar(SpaceStation station){
        CreateHangarIfNotExists(station);
        Dictionary<ResourceType, int> cost = hangar.GetUpgradeCost();
        RemoveResources(cost, station);
        hangar.Upgrade();
        station.HangarLevel = hangar.GetCurrentLevel();
        return true;
    }
    
    public HangarDTO UpdateHangarDTO(HangarDTO hangar)
    {
        hangar.FreeDocks += 1;
        return hangar;
    }
    
    public bool AddResourcesFromShip(MinerShipManager shipManager, Dictionary<ResourceType, int> resources, SpaceStation station) {
        if (HasShipAvailable(shipManager.GetShip(), station) && shipManager.HasResourcesInStorage(resources))
        { 
            int sum = resources.Values.Sum();
            CreateStorageIfNotExists( station);
            if (sum <= storage.GetCurrentAvailableStorageSpace()) {
                foreach (var resource in resources)
                {
                    AddResource( resource.Key, resource.Value, station);
                }
                return true;
            }
        }
        return false;
    }
    
    public bool UpgradeStorage(SpaceStation station) {
        CreateStorageIfNotExists(station);
        Dictionary<ResourceType, int> cost = storage.GetUpgradeCost();
        RemoveResources(cost, station);
        storage.Upgrade();
        station.StorageLevel = storage.GetCurrentLevel();
        return true;
    }

    public bool AddResource(ResourceType resourceType, int quantity, SpaceStation station)
    {
        CreateStorageIfNotExists( station);
        return storage.AddResource(resourceType, quantity);
    }

    public SpaceStationDTO GetStationDTO(SpaceStation station) {
        return new SpaceStationDTO(station.Id, station.Name, GetHangarDTO( station), GetStorageDTO(station));
    }
    
    public SpaceStationStorageDTO GetStorageDTO(SpaceStation station) {
        CreateStorageIfNotExists(station);
        return SpaceStationStorageDTOFactory.Create(storage);
    }

    public HangarDTO GetHangarDTO(SpaceStation station) { 
        CreateHangarIfNotExists(station);
        HangarDTO newHangar = HangarDTOFactory.Create(hangar);
        return newHangar;
    }

    public void CreateHangarIfNotExists(SpaceStation station)
    {
        if (hangar == null)
        {
            hangar = new HangarManager(levelService, station.HangarLevel, new HashSet<SpaceShip>(station.Hangar));
        }
    }

    public void CreateStorageIfNotExists(SpaceStation station)
    {
        if (storage == null)
        {
            var storedResourcesDictionary = station.StoredResources
                .ToDictionary(sr => sr.ResourceType, sr => sr.Amount);

            storage = new StationStorageManager(levelService, station.StorageLevel, storedResourcesDictionary);
        }
    }
}
