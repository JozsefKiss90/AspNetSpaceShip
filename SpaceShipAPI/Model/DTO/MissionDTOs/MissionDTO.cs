
using SpaceShipAPI.Model.Mission;

public class MissionDTO
{
    public long Id { get; }
    public string Title { get; }
    public MissionType Type { get; }
    public MissionStatus Status { get; }
    public DateTime CurrentObjectiveTime { get; }
    public DateTime ApproxEndTime { get; }

    public MissionDTO(Mission mission)
        : this(mission.Id, GenerateTitle(mission), GetMissionType(mission), mission.CurrentStatus,
            mission.CurrentObjectiveTime, mission.ApproxEndTime)
    {
    }

    public MissionDTO(long id, string title, MissionType type, MissionStatus status, DateTime currentObjectiveTime, DateTime approxEndTime)
    {
        Id = id;
        Title = title;
        Type = type;
        Status = status;
        CurrentObjectiveTime = currentObjectiveTime;
        ApproxEndTime = approxEndTime;
    }

    private static string GenerateTitle(Mission mission)
    {
        switch (mission)
        {
            case MiningMission miningMission:
                return $"Mining mission on {miningMission.Location.Name}";
            case ScoutingMission scoutingMission:
                return $"Exploration mission for {scoutingMission.TargetResource.ToString().ToLower()}";
            default:
                throw new InvalidOperationException("Mission type not recognized");
        }
    }

    private static MissionType GetMissionType(Mission mission)
    {
        return mission switch
        {
            MiningMission _ => MissionType.MINING,
            ScoutingMission _ => MissionType.SCOUTING,
            _ => throw new InvalidOperationException("Unrecognized mission type")
        };
    }
}
