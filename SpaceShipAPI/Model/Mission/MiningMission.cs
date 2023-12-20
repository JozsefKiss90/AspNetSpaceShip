using System.ComponentModel.DataAnnotations.Schema;
using SpaceShipAPI.Model.Location;
using SpaceShipAPI.Model.Mission;

[Table("Missions")]
public class MiningMission : Mission
{
    [ForeignKey("LocationId")]
    public Location Location { get; set; }
}
