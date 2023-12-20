using SpaceshipAPI.Model.Ship;

namespace SpaceShipAPI.Model.DTO.MissionDTOs;

public class ScoutingMissionDTO : MissionDetailDTO
{
    public ResourceType TargetResource { get; }
    public int Distance { get; }
    public bool PrioritizingDistance { get; }

    public ScoutingMissionDTO(long id, string title, MissionStatus status, DateTime currentObjectiveTime,
        DateTime approxEndTime, List<Event> events, SpaceShip ship, ResourceType targetResource, int distance, bool prioritizeDistance)
        : base(id, title, MissionType.SCOUTING, status, currentObjectiveTime, approxEndTime, events, ship)
    {
        TargetResource = targetResource;
        Distance = distance;
        PrioritizingDistance = prioritizeDistance;
    }

    public ScoutingMissionDTO(ScoutingMission mission)
        : this(mission.Id, GenerateMissionTitle(mission), mission.CurrentStatus, mission.CurrentObjectiveTime,
            mission.ApproxEndTime, mission.Events, mission.Ship, mission.TargetResource, mission.Distance, mission.PrioritizingDistance)
    {
    }

    private static string GenerateMissionTitle(ScoutingMission mission)
    {
        return $"Exploration mission for {mission.TargetResource.ToString().ToLower()}";
    }
}
