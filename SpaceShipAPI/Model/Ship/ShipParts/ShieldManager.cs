using SpaceShipAPI.Services;

namespace SpaceShipAPI.Model.Ship.ShipParts;

using SpaceShipAPI.Model;
using System;

public class ShieldManager : Upgradeable 
{
    private static readonly UpgradeableType TYPE = UpgradeableType.SHIELD;
    private int currentEnergy;

    public ShieldManager(ILevelService levelService, int currentLevel, int currentEnergy) 
        : base(levelService, TYPE, currentLevel)
    {
        if (currentEnergy < 0)
        {
            throw new ArgumentException("Shield energy can't be lower than 0");
        }
        if (currentEnergy > CurrentLevel.Effect) 
        {
            throw new Exception($"Shield energy can't be higher than {CurrentLevel.Effect} at this level");
        }
        this.currentEnergy = currentEnergy;
    }

    public ShieldManager(ILevelService levelService) 
        : base(levelService, TYPE, 1)
    {
        currentEnergy = CurrentLevel.Effect;
    }

    public int GetMaxEnergy()
    {
        return CurrentLevel.Effect;
    }

    public int GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public void SetEnergyToMax()
    {
        currentEnergy = GetMaxEnergy();
    }

    public void Repair(int amount)
    {
        currentEnergy = Math.Min(currentEnergy + amount, GetMaxEnergy());
    }

    public void Damage(int amount)
    {
        currentEnergy = Math.Max(currentEnergy - amount, 0);
    }
}
