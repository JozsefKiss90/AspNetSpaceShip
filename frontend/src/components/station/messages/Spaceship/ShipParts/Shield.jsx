export default function Shield({ shield, onClick }) {
  return (
    <div className="row">
      <div>
        Shield | lvl: {shield.level} | {shield.energy} / {shield.maxEnergy}
      </div>
      <button
        className="button"
        onClick={() => onClick("SHIELD")}
        disabled={shield.fullyUpgraded}
      >
        {shield.fullyUpgraded ? "Max level" : "Upgrade"}
      </button>
    </div>
  );
}
