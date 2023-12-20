using SpaceShipAPI.Model.DTO.Mission;
using SpaceShipAPI.Model.DTO.MissionDTOs;
using SpaceshipAPI.Model.Ship;

namespace SpaceShipAPI.Model.Mission;

public class MiningMissionDTO : MissionDetailDTO
{
    public string Location { get; }

    public MiningMissionDTO(long id, string title, MissionStatus status, DateTime currentObjectiveTime,
        DateTime approxEndTime, List<Event> events, SpaceShip ship, Location.Location location)
        : base(id, title, MissionType.MINING, status, currentObjectiveTime, approxEndTime, events, ship)
    {
        Location = location.Name; 
    }

    public MiningMissionDTO(MiningMission mission)
        : this(mission.Id, GenerateMissionTitle(mission), mission.CurrentStatus, mission.CurrentObjectiveTime,
            mission.ApproxEndTime, mission.Events, mission.Ship, mission.Location)
    {
    }

    private static string GenerateMissionTitle(MiningMission mission)
    {
        return $"Mining mission on {mission.Location.Name}";
    }
}
