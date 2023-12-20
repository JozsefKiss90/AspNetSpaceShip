using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpaceShipAPI.Model.Mission;
using SpaceshipAPI.Spaceship.Model.Station;

namespace SpaceshipAPI.Model.Ship

{
    [Table("spaceship")]
    public abstract class SpaceShip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public ShipColor Color { get; set; }
        public int EngineLevel { get; set; }
        public int ShieldLevel { get; set; }
        public int ShieldEnergy { get; set; }
    
        [ForeignKey("SpaceStationId")] 
        public long? SpaceStationId { get; set; }
        public SpaceStation SpaceStation { get; set; }
    
        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public UserEntity User { get; set; }
    
        [ForeignKey("CurrentMissionId")]
        public Mission? CurrentMission { get; set; }

        protected SpaceShip()
        {
        
        }

        protected SpaceShip(string name, ShipColor color, int engineLevel, int shieldLevel, int shieldEnergy)
        {
            Name = name;
            Color = color;
            EngineLevel = engineLevel;
            ShieldLevel = shieldLevel;
            ShieldEnergy = shieldEnergy;
        }
    }
}
