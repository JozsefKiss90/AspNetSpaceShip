namespace SpaceShipAPI.Model.DTO;

public record LocationDTO(
    long Id,
    string Name,
    ResourceType ResourceType,
    LocationReserve ResourceReserve,
    int DistanceFromStation,
    DateTime Discovered,
    long MissionId)
{
    public LocationDTO(Location.Location location) 
        : this(
            location.Id, 
            location.Name, 
            location.ResourceType, 
            AssignReserve(location.ResourceReserve),
            location.DistanceFromStation, 
            location.Discovered, 
            GetMissionId(location.CurrentMission))
    {
    }

    private static long GetMissionId(Model.Mission.Mission mission)
    {
        return mission?.Id ?? 0L;
    }

    private static LocationReserve AssignReserve(int reserve)
    {
        return reserve switch
        {
            <= 0 => LocationReserve.DEPLETED,
            <= 100 => LocationReserve.POOR,
            <= 500 => LocationReserve.MODERATE,
            <= 800 => LocationReserve.GOOD,
            _ => LocationReserve.RICH,
        };
    }
}
