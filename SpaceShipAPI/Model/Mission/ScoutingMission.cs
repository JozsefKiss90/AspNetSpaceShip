using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SpaceShipAPI;
using SpaceShipAPI.Model.Location;
using SpaceShipAPI.Model.Mission;


[Table("Missions")]
public class ScoutingMission : Mission
{
    [Column("TargetResource")]
    [EnumDataType(typeof(ResourceType))]
    public ResourceType TargetResource { get; set; }

    [Column("Distance")]
    public int Distance { get; set; }

    [Column("PrioritizingDistance")]
    public bool PrioritizingDistance { get; set; }

    [ForeignKey("DiscoveredLocationId")]
    public Location DiscoveredLocation { get; set; }

    public ScoutingMission()
    {
    }

    public ScoutingMission(ResourceType targetResource, int distance, bool prioritizingDistance, Location discoveredLocation)
    {
        TargetResource = targetResource;
        Distance = distance;
        PrioritizingDistance = prioritizingDistance;
        DiscoveredLocation = discoveredLocation;
    }
}
