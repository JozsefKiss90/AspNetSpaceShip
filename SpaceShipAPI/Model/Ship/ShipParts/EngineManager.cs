
using SpaceShipAPI.Services;
namespace SpaceShipAPI.Model.Ship.ShipParts;

public class EngineManager : Upgradeable
{
    private static readonly UpgradeableType TYPE = UpgradeableType.ENGINE;

    public EngineManager(ILevelService levelService, int currentLevel) 
        : base(levelService, TYPE, currentLevel)
    {
    }

    public EngineManager(ILevelService levelService) 
        : this(levelService, 1)
    {
    }

    public int Speed => CurrentLevel.Effect;
}
