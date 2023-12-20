import ShipResources from "./ShipResources";

export default function ShipStorage({ ship, setShip, onClick }) {
  const storage = ship.storage;
  const resourceSum = Object.values(storage.resources).reduce(
    (sum, next) => sum + next,
    0
  );
  return (
    <>
      <div className="row">
        <div>
          Storage | lvl: {storage.level} | {resourceSum} / {storage.maxCapacity}
        </div>
        <button
          className="button"
          onClick={() => onClick("STORAGE")}
          disabled={storage.fullyUpgraded}
        >
          {storage.fullyUpgraded ? "Max level" : "Upgrade"}
        </button>
      </div>
      <ShipResources ship={ship} setShip={setShip} />
    </>
  );
}
