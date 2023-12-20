using SpaceShipAPI.Model.DTO.Ship;
using SpaceShipAPI.Model.DTO.Ship.Part;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Repository;
using SpaceShipAPI.Services;
using SpaceShipAPI.Utils;

namespace SpaceShipAPI.Model.Ship;

using System;
using System.Collections.Generic;

public class MinerShipManager : SpaceShipManager, IMinerShipManager
{
    private static readonly HashSet<ShipPart> PARTS = new HashSet<ShipPart> { ShipPart.ENGINE, ShipPart.SHIELD, ShipPart.DRILL, ShipPart.STORAGE };
    private readonly MinerShip minerShip;
    private ShipStorageManager storage;
    private DrillManager drill;

    public MinerShipManager(ILevelService levelService, MinerShip minerShip) 
        : base(levelService, minerShip)
    {
        this.minerShip = minerShip;
    } 
    
    public override SpaceShip CreateNewShip(ILevelService levelService, string name, ShipColor color)
    {
        return CreateNewMinerShip(levelService, name, color);
    }


    public static MinerShip CreateNewMinerShip(ILevelService levelService, string name, ShipColor color)
    {
        MinerShip ship = new MinerShip 
        {
            Name = name,
            Color = color,
            EngineLevel = 1,
            ShieldLevel = 1,
            ShieldEnergy = new ShieldManager(levelService, 1, 1).GetMaxEnergy(),
            DrillLevel = 1,
            StorageLevel = 1,
            StoredResources = new List<StoredResource>(),
        };
        return ship;
    }
   
    public override ShipDetailDTO GetDetailedDTO()
    {
        CreateEngineIfNotExists(); 
        CreateShieldIfNotExists();
        CreateDrillIfNotExists();
        CreateStorageIfNotExists();

        var engineDto = new EngineDTO(engine);
        var shieldDto = new ShieldDTO(shield);
        var drillDto = new DrillDTO(drill);
        var storageDto = new ShipStorageDTO(storage);

        return new MinerShipDTO(minerShip.Id, minerShip.Name, minerShip.Color, ShipType.MINER, 
            minerShip.CurrentMission, engineDto, shieldDto, drillDto, storageDto);
    }
    

    public override HashSet<ShipPart> GetPartTypes()
    {
        return PARTS;
    }

    public override Dictionary<ResourceType, int> GetUpgradeCost(ShipPart part)
    {
        switch (part)
        {
            case ShipPart.ENGINE:
                CreateEngineIfNotExists();
                return engine.GetUpgradeCost();
            case ShipPart.SHIELD:
                CreateShieldIfNotExists();
                return shield.GetUpgradeCost();
            case ShipPart.DRILL:
                CreateDrillIfNotExists();
                return drill.GetUpgradeCost();
            case ShipPart.STORAGE:
                CreateStorageIfNotExists();
                return storage.GetUpgradeCost();
            default:
                throw new Exception("No such part on this ship");
        }
    }
    
    public int GetDrillEfficiency() {
        CreateDrillIfNotExists();
        return drill.Efficiency;
    }
    
    public int GetEmptyStorageSpace() {
        CreateStorageIfNotExists();
        return storage.GetCurrentAvailableStorageSpace();
    }
    
    public bool AddResourceToStorage(ResourceType resourceType, int amount) {
        CreateStorageIfNotExists();
        return storage.AddResource(resourceType, amount);
    }

    public override bool UpgradePart(ShipPart part)
    {
        switch (part)
        {
            case ShipPart.ENGINE:
                CreateEngineIfNotExists();
                engine.Upgrade();
                minerShip.EngineLevel = engine.GetCurrentLevel();
                break;
            case ShipPart.SHIELD:
                CreateShieldIfNotExists();
                shield.Upgrade();
                shield.SetEnergyToMax();
                minerShip.ShieldLevel = shield.GetCurrentLevel();
                minerShip.ShieldEnergy = shield.GetCurrentEnergy();
                break;
            case ShipPart.DRILL:
                CreateDrillIfNotExists();
                drill.Upgrade();
                minerShip.DrillLevel = drill.GetCurrentLevel();
                break;
            case ShipPart.STORAGE:
                CreateStorageIfNotExists(); 
                storage.Upgrade();
                minerShip.StorageLevel = storage.GetCurrentLevel();
                break;
            default:
                throw new Exception("No such part on this ship");
        }
        return true;
    }

    public override Dictionary<ResourceType, int> GetCost()
    {
        return new Dictionary<ResourceType, int>(ShipType.MINER.GetCost());
    }
    
    
    public bool HasResourcesInStorage(Dictionary<ResourceType, int> resources) {
        CreateStorageIfNotExists(); 
        bool allMatch = resources.All(entry => storage.HasResource(entry.Key, entry.Value));
        return allMatch;
    }

    private void CreateStorageIfNotExists()
    {
        if (storage == null)
        {
            var storedResources = minerShip.StoredResources ?? new List<StoredResource>();
            var storedResourcesDictionary = ResourceUtility.ConvertToDictionary(storedResources);
            storage = new ShipStorageManager(levelService, minerShip.StorageLevel, storedResourcesDictionary);
        }
    }


    private void CreateDrillIfNotExists()
    {
        if (drill == null)
        {
            drill = new DrillManager(levelService, minerShip.DrillLevel);
        }
    }
}
