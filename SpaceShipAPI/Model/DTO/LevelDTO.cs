using SpaceShipAPI.Model;
using System.Collections.Generic;

namespace SpaceShipAPI.DTO
{
    public record LevelDTO(long Id, UpgradeableType Type, int LevelValue, int Effect, Dictionary<ResourceType, int> Cost, bool Max)
    {
        public LevelDTO(Level level) 
            : this(
                level.Id, 
                level.Type, 
                level.LevelValue, 
                level.Effect, 
                level.Costs.ToDictionary(lc => lc.Resource, lc => lc.Amount), // Convert ICollection<LevelCost> to Dictionary
                level.Max)
        {
        }
    }
}