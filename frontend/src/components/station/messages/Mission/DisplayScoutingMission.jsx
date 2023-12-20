export default function DisplayScoutingMission({ mission }) {
  const formattedStatus = mission.status.replaceAll("_", " ");

  return (
    <>
      <div className="mission-data">
        <div>
          <div>Target resource</div>
          <div>{mission.targetResource}</div>
        </div>
        <div className="mission-status">{formattedStatus}</div>
        <div>
          <div>Ship</div>
          <div>{mission.shipName}</div>
        </div>
      </div>
      <div className="mission-data mission-scout-data">
        <div>
          <div>Scanning distance</div>
          <div>{mission.distance}</div>
        </div>
        <div>
          <div>Priority</div>
          <div>{mission.prioritizingDistance ? "Closeness" : "Resource abundance"}</div>
        </div>
      </div>
    </>
  );
}
