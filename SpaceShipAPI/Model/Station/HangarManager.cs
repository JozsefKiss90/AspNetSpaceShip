using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Services;

namespace SpaceshipAPI.Spaceship.Model.Station;

using SpaceShipAPI.Model;
using SpaceShipAPI.Model.Exceptions;
using System;
using System.Collections.Generic;

public class HangarManager : Upgradeable, IHangarManager
{
    private static readonly UpgradeableType Type = UpgradeableType.HANGAR;
    private readonly HashSet<SpaceShip> shipSet;

    public HangarManager(ILevelService levelService, int currentLevel, HashSet<SpaceShip> shipSet) 
        : base(levelService, Type, currentLevel)
    {
        if (shipSet == null)
        {
            throw new ArgumentNullException(nameof(shipSet));
        }

        if (CurrentLevel == null || shipSet.Count > CurrentLevel.Effect)
        {
            throw new InvalidOperationException($"Current level is not properly initialized.");
        }

        this.shipSet = shipSet;
    }

    public HangarManager(ILevelService levelService) 
        : this(levelService, 1, new HashSet<SpaceShip>())
    {
    }

    public int GetCurrentCapacity()
    {
        return CurrentLevel.Effect;
    }

    public int GetCurrentAvailableDocks()
    {
        return GetCurrentCapacity() - shipSet.Count;
    }

    public bool AddShip(SpaceShip ship)
    {
        if (GetCurrentAvailableDocks() > 0)
        {
            return shipSet.Add(ship);
        }
        throw new Exception("No more docks available");
    }

    public bool RemoveShip(SpaceShip ship)
    {
        return shipSet.Remove(ship);
    }

    public HashSet<SpaceShip> GetAllShips()
    {
        return new HashSet<SpaceShip>(shipSet); 
    }

    public bool HasShipAvailable(SpaceShip ship)
    {
        if (!shipSet.Contains(ship))
        {
            throw new Exception("No such ship in storage");
        }
        else if (ship.CurrentMission != null)
        {
            throw new Exception("Ship is on a mission");
        }
        else
        {
            return true;
        }
    }
}
