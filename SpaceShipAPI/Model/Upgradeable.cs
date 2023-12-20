using SpaceShipAPI.Model.Exceptions;
using SpaceShipAPI.Repository;
using SpaceShipAPI.Services;

namespace SpaceShipAPI.Model;

public abstract class Upgradeable
{
    private readonly ILevelService levelService;
    protected Level CurrentLevel;

    protected Upgradeable(ILevelService levelService, UpgradeableType type, int level)
    {
        this.levelService = levelService;
        CurrentLevel = levelService.GetLevelByTypeAndLevel(type, level);
        CurrentLevel.Effect = 100; //line 16
    }

    public bool IsFullyUpgraded()
    {
        return CurrentLevel.Max;
    }
    
    public Dictionary<ResourceType, int> GetUpgradeCost()
    {
        if (CurrentLevel == null)
        {
            throw new Exception("CurrentLevel is null");
        }

        if (IsFullyUpgraded())
        {
            throw new UpgradeNotAvailableException("Already at max level");        }

        int level = CurrentLevel.LevelValue;

        UpgradeableType type = CurrentLevel.Type;
        Level nextLevel = levelService.GetLevelByTypeAndLevel(type, level + 1);

        if (nextLevel == null)
        {
            throw new Exception("NextLevel is null");
        }

        return nextLevel.Costs.ToDictionary(cost => cost.Resource, cost => cost.Amount);
    }

    public bool Upgrade()
    {
        if (IsFullyUpgraded())
        { throw new Exception("Already at max level");
        }
        else
        {
            int level = CurrentLevel.LevelValue;
            UpgradeableType type = CurrentLevel.Type;
            CurrentLevel = levelService.GetLevelByTypeAndLevel(type, level + 1);
            Console.WriteLine(CurrentLevel.Effect);
            return true;
        }
    }

    public int GetCurrentLevel()
    {
        return CurrentLevel.LevelValue;
    }
}
