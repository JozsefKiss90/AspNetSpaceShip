using SpaceShipAPI.Model.Mission;

namespace SpaceShipAPI.Model.Ship.ShipParts;

public interface IMissionFactory
{
    MiningMission StartNewMiningMission(MinerShip spaceShip, Location.Location location, long activityDurationInSecs);

    ScoutingMission StartNewScoutingMission(ScoutShip spaceShip, int distance, long activityDurationInSecs,
        ResourceType targetResource, bool prioritizeDistance);

    MissionManager GetMissionManager(Mission.Mission mission);
}