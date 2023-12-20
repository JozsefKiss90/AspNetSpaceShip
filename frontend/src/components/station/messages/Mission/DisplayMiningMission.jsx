export default function DisplayMiningMission({ mission }) {
  const formattedStatus = mission.status.replaceAll("_", " ");

  return (
    <div className="mission-data">
      <div>
        <div>Location</div>
        <div>{mission.location}</div>
      </div>
      <div className="mission-status">{formattedStatus}</div>
      <div>
        <div>Ship</div>
        <div>{mission.shipName}</div>
      </div>
    </div>
  );
}
