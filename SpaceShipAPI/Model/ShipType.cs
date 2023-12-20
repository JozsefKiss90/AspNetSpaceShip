namespace SpaceShipAPI.Model.Ship
{
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ShipType
    {
        MINER,
        SCOUT
    }

    public static class ShipTypeExtensions
    {
        private static readonly Dictionary<ShipType, Dictionary<ResourceType, int>> Costs = new Dictionary<ShipType, Dictionary<ResourceType, int>>
        {
            {
                ShipType.MINER, new Dictionary<ResourceType, int>
                {
                    { ResourceType.METAL, 50 },
                    { ResourceType.CRYSTAL, 20 },
                    { ResourceType.SILICONE, 20 }
                }
            },
            {
                ShipType.SCOUT, new Dictionary<ResourceType, int>
                {
                    { ResourceType.METAL, 10 },
                    { ResourceType.CRYSTAL, 10 },
                    { ResourceType.SILICONE, 10 }
                }
            }
        };

        public static Dictionary<ResourceType, int> GetCost(this ShipType shipType)
        {
            return Costs[shipType];
        }
    }
}