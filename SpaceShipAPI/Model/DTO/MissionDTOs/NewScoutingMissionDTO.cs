namespace SpaceShipAPI.Model.DTO.MissionDTO;

public record NewScoutingMissionDTO(long ShipId, int Distance, long ActivityTime, ResourceType TargetResource, bool PrioritizingDistance)
{
    // Constructor and property declarations
}
