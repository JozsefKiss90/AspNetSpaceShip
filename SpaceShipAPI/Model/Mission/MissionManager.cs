using System;
using System.Collections.Generic;
using SpaceShipAPI.Model.DTO.Mission;
using SpaceShipAPI.Model.DTO.MissionDTOs;
using SpaceShipAPI.Model.Mission;
using SpaceShipAPI.Model.Ship;

namespace SpaceShipAPI.Model.Mission;
public abstract class MissionManager
{
    protected readonly Mission mission;

    protected readonly Random random;
    protected ISpaceShipManager shipManager;
    protected readonly DateTime clock;
    protected MissionManager(Mission mission, Random random, ISpaceShipManager spaceShipManager)
    {
        this.mission = mission;
        this.random = random;
        this.clock = clock;
        if (!mission.Ship.Equals(spaceShipManager.GetShip())) 
        {
            throw new ArgumentException("Ship is not the one on this mission.");
        }
        this.shipManager = spaceShipManager;
    }

    public void SetShipManager(ISpaceShipManager spaceShipManager)
    {
        if (!mission.Ship.Equals(spaceShipManager.GetShip()))
        {
            throw new ArgumentException("Ship is not the one on this mission.");
        }
        this.shipManager = spaceShipManager;
    }

    public abstract MissionDetailDTO GetDetailedDTO();

    public bool UpdateStatus()
    {
        if (mission.Events.Count == 0)
        {
            AddStartEvent();
        }

        var lastEvent = PeekLastEvent();
        if (mission.CurrentStatus == MissionStatus.OVER ||
            mission.CurrentStatus == MissionStatus.ARCHIVED ||
            lastEvent.EndTime > DateTime.Now)  // Assuming clock was UTC
        {
            return false;
        }

        switch (lastEvent.EventType)
        {
            case EventType.START:
                GenerateEnRouteEvents();
                break;
            case EventType.ARRIVAL_AT_LOCATION:
                StartActivity();
                break;
            case EventType.ACTIVITY_COMPLETE:
                FinishActivity();
                break;
            case EventType.RETURNED_TO_STATION:
                EndMission();
                break;
            default:
                throw new InvalidOperationException("Unknown activity type");
        }

        UpdateStatus();
        return true;
    }

    public abstract bool AbortMission();  // IllegalOperationException should be defined in C#

    protected abstract void AddStartEvent();

    protected abstract void StartActivity();

    protected abstract void FinishActivity();

    protected abstract void EndMission();

    protected void StartReturnTravel()
    {
        var lastEventTime = PeekLastEvent().EndTime;
        long returnDurationInSecs;
        if (mission.CurrentStatus == MissionStatus.EN_ROUTE)
        {
            returnDurationInSecs = (long)(lastEventTime - mission.StartTime).TotalSeconds;
        }
        else
        {
            returnDurationInSecs = mission.TravelDurationInSecs;
        }

        mission.CurrentObjectiveTime = lastEventTime.AddSeconds(returnDurationInSecs);
        mission.CurrentStatus = MissionStatus.RETURNING;
        GenerateEnRouteEvents();
    }

    public bool ArchiveMission()
    {
        if (mission.CurrentStatus == MissionStatus.ARCHIVED)
        {
            throw new Exception("Mission is already archived");
        }
        else if (mission.CurrentStatus != MissionStatus.OVER)
        {
            throw new Exception("Mission can't be archived until it's over");
        }
        else
        {
            mission.CurrentStatus = MissionStatus.ARCHIVED;
            return true;
        }
    }

    protected void GenerateEnRouteEvents()
    {
        //TODO: Add pirate attack/meteor storm based on chance
        var travelEventType = (mission.CurrentStatus == MissionStatus.EN_ROUTE)
            ? EventType.ARRIVAL_AT_LOCATION
            
            : EventType.RETURNED_TO_STATION;

        var travelEvent = new Event
        {
            EndTime = mission.CurrentObjectiveTime,
            EventType = travelEventType
        };

        PushNewEvent(travelEvent);
    }
    
    protected static long CalculateTravelDurationInSecs(SpaceShipManager ship, int distanceFromBase)
    {
        double speedInDistancePerHour = ship.GetSpeed();
        int hourToSec = 3600; 
        return (long)(distanceFromBase / speedInDistancePerHour * hourToSec);
    }
    
    protected Event PeekLastEvent()
    {
        var events = mission.Events.ToList(); 
        return events[events.Count - 1];
    }
    
    protected bool PushNewEvent(Event @event)
    {
        var events = mission.Events;
        events.Add(@event);
        return true; // Always returns true as List.Add() in C# doesn't return a boolean
    }
        
    protected Event PopLastEvent()
    {
        var events = mission.Events.ToList(); // Convert to List
        if (events.Count == 0)
        {
            throw new InvalidOperationException("No events to pop.");
        }

        var eventToPop = events[events.Count - 1];
        events.RemoveAt(events.Count - 1);
        mission.Events = events; // Reassign the modified list back to the ICollection
        return eventToPop;
    }


}   
