namespace SpaceShipAPI.Model;

public interface IUpgradeableManager
{
    bool Upgrade();
    int GetCurrentLevel();
    bool IsFullyUpgraded();
    Dictionary<ResourceType, int> GetUpgradeCost();
}