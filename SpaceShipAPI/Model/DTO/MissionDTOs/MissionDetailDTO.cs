using SpaceShipAPI.Model.DTO.Mission;
using SpaceshipAPI.Model.Ship;

namespace SpaceShipAPI.Model.DTO.MissionDTOs;

public abstract class MissionDetailDTO : global::MissionDTO
{
    public List<EventDTO> Reports { get; }
    public long ShipId { get; }
    public string ShipName { get; }

    public MissionDetailDTO(long id, string title, MissionType missionType, MissionStatus status, DateTime currentObjectiveTime,
        DateTime approxEndTime, List<Event> events, SpaceShip ship)
        : base(id, title, missionType, status, currentObjectiveTime, approxEndTime)
    {
        Reports = FormatEventLog(events);
        ShipId = ship.Id;
        ShipName = ship.Name;
    }

    private static List<EventDTO> FormatEventLog(ICollection<Event> events)
    {
        return events.Where(e => e.EventMessage != null)
            .Select(e => new EventDTO(e.EndTime, e.EventMessage))
            .ToList();
    }
}
