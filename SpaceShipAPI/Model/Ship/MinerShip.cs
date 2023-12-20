using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SpaceshipAPI.Model.Ship;
using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Services;

namespace SpaceShipAPI.Model.Ship
{
    [Table("minership")]
    public class MinerShip : SpaceShip
    {
        public int DrillLevel { get; set; }
        public int StorageLevel { get; set; }
        public ICollection<StoredResource>? StoredResources { get; set; }
        
        public static MinerShip CreateNewMinerShip(ILevelService levelService, string name, ShipColor color)
        {
            return new MinerShip
            {
                Name = name,
                Color = color,
                EngineLevel = 1,
                ShieldLevel = 1,
                ShieldEnergy = new ShieldManager(levelService, 1, 0).GetMaxEnergy(),
                DrillLevel = 1,
                StorageLevel = 1,
                StoredResources = new List<StoredResource>()
            };
        }
    }

}