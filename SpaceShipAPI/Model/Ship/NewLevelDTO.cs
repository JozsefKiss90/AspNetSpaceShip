using SpaceShipAPI;
using SpaceShipAPI.Model;

public record NewLevelDTO(UpgradeableType Type, int Effect, Dictionary<ResourceType, int> Cost);
