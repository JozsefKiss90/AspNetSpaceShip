export default function Engine({ engine, onClick }) {
  return (
    <div className="row">
      <div>
        Engine | lvl: {engine.level} | AU/hour: {engine.speed}
      </div>
      <button
        className="button"
        onClick={() => onClick("ENGINE")}
        disabled={engine.fullyUpgraded}
      >
        {engine.fullyUpgraded ? "Max level" : "Upgrade"}
      </button>
    </div>
  );
}
