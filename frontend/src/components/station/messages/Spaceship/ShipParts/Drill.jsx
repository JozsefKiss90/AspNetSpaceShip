export default function Drill({ drill, onClick }) {
  return (
    <div className="row">
      <div>
        Drill | lvl: {drill.level} | resource/hour: {drill.efficiency}
      </div>
      <button
        className="button"
        onClick={() => onClick("DRILL")}
        disabled={drill.fullyUpgraded}
      >
        {drill.fullyUpgraded ? "Max level" : "Upgrade"}
      </button>
    </div>
  );
}
