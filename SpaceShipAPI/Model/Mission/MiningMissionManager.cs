using SpaceShipAPI.Model.Ship;
using SpaceShipAPI.Model.Location;
using SpaceShipAPI.Model.DTO.Mission;
using SpaceShipAPI.Model.DTO.MissionDTOs;
using SpaceShipAPI.Model.Mission;

public class MiningMissionManager : MissionManager
{
    public MiningMissionManager(MiningMission mission, Random random, MinerShipManager minerShipManager)
        : base(mission, random, minerShipManager)
    {
    }
    
    public MiningMissionManager(MiningMission mission, DateTime clock, MinerShipManager minerShipManager)
        : this(mission, new Random(), minerShipManager)
    {
    }

    public static MiningMission StartMiningMission(MinerShipManager minerShipManager, Location location, long activityDurationInSecs, DateTime clock)
    {
        if (!minerShipManager.IsAvailable())
        {
            throw new Exception("This ship is already on a mission");
        }
        if (location.CurrentMission != null)
        {
            throw new Exception("There is a mission already in progress at this location");
        }
        if (activityDurationInSecs <= 0)
        {
            throw new ArgumentException("Activity duration can't be 0 or less");
        }

        var startTime = DateTime.Now;
        long travelDurationInSecs = CalculateTravelDurationInSecs(minerShipManager, location.DistanceFromStation);
        long approxMissionDurationInSecs = travelDurationInSecs * 2 + activityDurationInSecs;

        MiningMission mission = new MiningMission
        {
            StartTime = startTime,
            ActivityDurationInSecs = activityDurationInSecs,
            TravelDurationInSecs = travelDurationInSecs,
            CurrentStatus = MissionStatus.EN_ROUTE,
            CurrentObjectiveTime = startTime.AddSeconds(travelDurationInSecs), 
            ApproxEndTime = startTime.AddSeconds(approxMissionDurationInSecs), 
            Ship = minerShipManager.GetShip(),
            Location = location,
            User = minerShipManager.GetShip().User,
            Events = new List<Event>()
        };
        minerShipManager.SetCurrentMission(mission);
        location.CurrentMission = mission;
        return mission;
    }

    public static MiningMission StartMiningMission(MinerShipManager minerShipManager, Location location, long activityDurationInSecs)
    {
        return StartMiningMission(minerShipManager, location, activityDurationInSecs, DateTime.UtcNow);
    }

    public override MissionDetailDTO GetDetailedDTO()
    {
        return new MiningMissionDTO((MiningMission)mission);
    }
    

    private void SimulatePirateAttack()
    {
        //TODO: Implement simulation logic
    }

    private void SimulateMeteorStorm()
    {
        //TODO: Implement simulation logic
    }
    
    protected override void FinishActivity()
    {
        var minerShipManager = (MinerShipManager)shipManager;
        int minedResources = CalculateMinedResources();

        var location = ((MiningMission)mission).Location;
        var resourceType = location.ResourceType;
        location.ResourceReserve -= minedResources;
        minerShipManager.AddResourceToStorage(resourceType, minedResources);

        string message;
        if (location.ResourceReserve <= 0)
        {
            message = $"Planet depleted. Mined {minedResources} {resourceType}(s). Returning to station.";
        }
        else if (minerShipManager.GetEmptyStorageSpace() == 0)
        {
            message = $"Storage is full. Mined {minedResources} {resourceType}(s). Returning to station.";
        }
        else
        {
            message = $"Mining complete. Mined {minedResources} {resourceType}(s). Returning to station.";
        }

        PeekLastEvent().EventMessage = message;
        mission.ApproxEndTime = PeekLastEvent().EndTime.AddSeconds(mission.TravelDurationInSecs);
        StartReturnTravel();
    }


    protected override void EndMission()
    {
        PeekLastEvent().EventMessage = "Returned to station.";
        mission.CurrentStatus = MissionStatus.OVER;
        shipManager.EndMission();
        ((MiningMission)mission).Location.CurrentMission = null;
    }


    public override bool AbortMission() 
    {
        switch (mission.CurrentStatus)
        {
            case MissionStatus.OVER:
            case MissionStatus.ARCHIVED:
                throw new Exception("Mission is already over.");
            case MissionStatus.RETURNING:
                throw new Exception("Mission is already returning.");
        }

        var now = DateTime.UtcNow;  
        var abortedEvent = PopLastEvent();
        var abortEvent = new Event
        {
            EventType = EventType.ABORT,
            EndTime = now
        };

        if (abortedEvent.EventType == EventType.ACTIVITY_COMPLETE)
        {
            var minerShipManager = (MinerShipManager)shipManager;
            var updatedActivityTime = Convert.ToInt64((now - PeekLastEvent().EndTime).TotalSeconds);
            mission.ActivityDurationInSecs = updatedActivityTime; 

            int minedResources = CalculateMinedResources();
            var location = ((MiningMission)mission).Location;
            location.ResourceReserve -= minedResources;
            var resourceType = location.ResourceType;
            minerShipManager.AddResourceToStorage(resourceType, minedResources);

            abortEvent.EventMessage = $"Mission aborted by Command. Mined {minedResources} {resourceType}(s). Returning to station.";
        }
        else
        {
            abortEvent.EventMessage = "Mission aborted by Command. Returning to station.";
        }

        PushNewEvent(abortEvent);
        mission.ApproxEndTime = now.AddSeconds(mission.TravelDurationInSecs);
        StartReturnTravel();
        return true;
    }

    protected override void AddStartEvent()
    {
        var startEvent = new Event
        {
            EndTime = mission.StartTime,
            EventType = EventType.START,
            EventMessage = $"Left station for mining mission on {((MiningMission)mission).Location.Name}."
        };
        PushNewEvent(startEvent);
    }

    protected override void StartActivity()
    {
        mission.CurrentStatus = MissionStatus.IN_PROGRESS;

        var lastEventTime = PeekLastEvent().EndTime;
        mission.CurrentObjectiveTime = lastEventTime.AddSeconds(mission.ActivityDurationInSecs);
        var location = ((MiningMission)mission).Location;
        PeekLastEvent().EventMessage = $"Arrived on {location.Name}. Starting mining operation.";
        var miningDurationInSecs = CalculateMiningDurationInSecs();
        mission.ActivityDurationInSecs = miningDurationInSecs;
        var activityEvent = new Event
        {
            EventType = EventType.ACTIVITY_COMPLETE,
            EndTime = lastEventTime.AddSeconds(miningDurationInSecs)
        };
        PushNewEvent(activityEvent);
    }
    
    private long CalculateMiningDurationInSecs()
    {
        var minerShipManager = (MinerShipManager)shipManager;
        var resourceReserve = ((MiningMission)mission).Location.ResourceReserve;
        var resourceMinedPerHour = minerShipManager.GetDrillEfficiency();
        var resourceMinedInSetTime = CalculateMinedResources();
        var emptyStorageSpace = minerShipManager.GetEmptyStorageSpace();

        if (resourceMinedInSetTime <= emptyStorageSpace && resourceMinedInSetTime <= resourceReserve)
        {
            return mission.ActivityDurationInSecs;
        }
        else if (emptyStorageSpace <= resourceReserve)
        {
            double hoursNeededToFillStorage = (double)emptyStorageSpace / resourceMinedPerHour;
            return (long)Math.Ceiling(hoursNeededToFillStorage * 3600); // Convert hours to seconds
        }
        else
        {
            double hoursNeededToMineAllResources = (double)resourceReserve / resourceMinedPerHour;
            return (long)Math.Ceiling(hoursNeededToMineAllResources * 3600); // Convert hours to seconds
        }
    }

    private int CalculateMinedResources()
    {
        var resourceMinedPerHour = ((MinerShipManager)shipManager).GetDrillEfficiency();
        var activityDurationInHours = mission.ActivityDurationInSecs / 3600.0;
        return (int)Math.Floor(resourceMinedPerHour * activityDurationInHours);
    }
}
