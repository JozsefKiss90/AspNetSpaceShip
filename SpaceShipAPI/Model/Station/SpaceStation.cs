using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpaceShipAPI;
using SpaceshipAPI.Model.Ship;

namespace SpaceshipAPI.Spaceship.Model.Station
{
    [Table("spacestation")]
    public class SpaceStation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public int StorageLevel { get; set; }
        public int HangarLevel { get; set; }
        
        [InverseProperty("SpaceStation")]
        public ICollection<SpaceShip>? Hangar { get; set; }

        [InverseProperty("SpaceStation")]
        public ICollection<StoredResource>? StoredResources { get; set; }

        [ForeignKey("UserId")] 
        public string? UserId { get; set; }
        public UserEntity User { get; set; }
    }
}
