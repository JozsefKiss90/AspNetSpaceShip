using SpaceShipAPI.Services;

namespace SpaceShipAPI.Model.Ship.ShipParts;

public class DrillManager : Upgradeable
{
    private static readonly UpgradeableType TYPE = UpgradeableType.DRILL;

    public DrillManager(ILevelService levelService, int currentLevel) 
        : base(levelService, TYPE, currentLevel)
    {
    }

    public DrillManager(ILevelService levelService) 
        : this(levelService, 1)
    {
    }

    public int Efficiency => CurrentLevel.Effect;
}
