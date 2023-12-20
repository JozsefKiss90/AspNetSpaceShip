using SpaceShipAPI.Model.DTO.Ship;
using SpaceShipAPI.Model.DTO.Ship.Part;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Services;

namespace SpaceShipAPI.Model.Ship;

using System;
using System.Collections.Generic;
using SpaceShipAPI.Model;

public class ScoutShipManager : SpaceShipManager
{
    private static readonly HashSet<ShipPart> PARTS = new HashSet<ShipPart> { ShipPart.ENGINE, ShipPart.SHIELD, ShipPart.SCANNER };
    private readonly ScoutShip scoutShip;
    private ScannerManager scanner;
    
    public ScoutShipManager(ILevelService levelService, ScoutShip scoutShip) 
        : base(levelService, scoutShip)
    {
        this.scoutShip = scoutShip;
    }

    public static ScoutShip CreateNewScoutShip(ILevelService levelService, string name, ShipColor color)
    {
        ScoutShip ship = new ScoutShip
        {
            Name = name,
            Color = color,
            EngineLevel = 1,
            ShieldLevel = 1,
            ShieldEnergy = new ShieldManager(levelService, 1, 0).GetMaxEnergy(),
            ScannerLevel = 1
        };
        return ship;
    }

    public int GetScannerEfficiency()
    {
        CreateScannerIfNotExists();
        return scanner.Efficiency;
    }

    public override SpaceShip CreateNewShip(ILevelService levelService, string name, ShipColor color)
    {
        throw new NotImplementedException();
    }

    public override ShipDetailDTO GetDetailedDTO()
    {
        CreateEngineIfNotExists();
        CreateShieldIfNotExists(); 
        CreateScannerIfNotExists();
        return new ScoutShipDTO(
            scoutShip.Id,
            scoutShip.Name,
            scoutShip.Color,
            ShipType.SCOUT,
            scoutShip.CurrentMission, 
            new EngineDTO(engine),
            new ShieldDTO(shield),
            new ScannerDTO(scanner)
        );
    }

    // Other methods...

    public override HashSet<ShipPart> GetPartTypes()
    {
        return PARTS;
    }

    public override bool UpgradePart(ShipPart part)
    {
        switch (part)
        {
            case ShipPart.ENGINE:
                CreateEngineIfNotExists();
                engine.Upgrade();
                scoutShip.EngineLevel = engine.GetCurrentLevel();
                break;
            case ShipPart.SHIELD:
                CreateShieldIfNotExists();
                shield.Upgrade();
                shield.SetEnergyToMax();
                scoutShip.ShieldLevel = shield.GetCurrentLevel();
                scoutShip.ShieldEnergy = shield.GetCurrentEnergy();
                break;
            case ShipPart.SCANNER:
                CreateScannerIfNotExists();
                scanner.Upgrade();
                scoutShip.ScannerLevel = scanner.GetCurrentLevel();
                break;
            default:
                throw new Exception("No such part on this ship");
        }
        return true;
    }

    public override Dictionary<ResourceType, int> GetCost()
    {
        return new Dictionary<ResourceType, int>(ShipType.SCOUT.GetCost());
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

            case ShipPart.SCANNER:
                CreateScannerIfNotExists();
                return scanner.GetUpgradeCost();

            default:
                throw new Exception("No such part on this ship");
        }
    }


    private void CreateScannerIfNotExists()
    {
        if (scanner == null)
        {
            scanner = new ScannerManager(levelService, scoutShip.ScannerLevel);
        }
    }
}
