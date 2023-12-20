using SpaceshipAPI.Model.Ship;

using System.ComponentModel.DataAnnotations.Schema;
using SpaceShipAPI.Model.Ship.ShipParts;
using SpaceShipAPI.Services;

namespace SpaceShipAPI.Model.Ship
{
    [Table("scoutship")]
    public class ScoutShip : SpaceShip
    {
        public int ScannerLevel { get; set; }

        public static ScoutShip CreateNewScoutShip(ILevelService levelService, string name, ShipColor color)
        {
            return new ScoutShip
            {
                Name = name,
                Color = color,
                EngineLevel = 1,
                ShieldLevel = 1,
                ShieldEnergy = new ShieldManager(levelService, 1, 0).GetMaxEnergy(),
                ScannerLevel = 1
            };
        }
    }

}