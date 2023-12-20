using SpaceshipAPI;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpaceShipAPI;
using SpaceShipAPI.Model.Mission;

namespace SpaceShipAPI.Model.Location;
public class Location
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime Discovered { get; set; }
    public int DistanceFromStation { get; set; }

    [Column(TypeName = "varchar(255)")]
    public ResourceType ResourceType { get; set; }
    public int ResourceReserve { get; set; }
    
    [ForeignKey("CurrentMissionId")]
    public Mission.Mission CurrentMission { get; set; }

    [ForeignKey("UserId")]
    public UserEntity User { get; set; }
}
