import { useEffect, useMemo, useState } from "react";
import ResourceList from "../../../ResourceList";
import MoveResources from "./MoveResources";
import { useStorageDispatchContext } from "../../../../StorageContext";
import "./ShipResources.css";

export default function ShipResources({ ship, setShip }) {
  const [moveMode, setMoveMode] = useState(false);
  const storageSetter = useStorageDispatchContext();

  useEffect(() => {
    setMoveMode(false);
  }, [ship]);

  function applyMove(moveAmount) {
    storageSetter({ type: "update" });
    const newShip = { ...ship };
    for (const resource of Object.keys(moveAmount)) {
      const resources = newShip.storage.resources;
      resources[resource] -= moveAmount[resource];
      if (resources[resource] === 0) {
        delete resources[resource];
      }
    }
    setShip(newShip);
    setMoveMode(false);
  }

  const resources = useMemo(() => {
    const resources = {...ship.storage.resources};
    for (const resource in resources) {
      if (resources[resource] < 1) {
        delete resources[resource];
      }
    }
    return resources;
  },[ship]);

  return (
    <div className="row" style={{ alignItems: "unset" }}>
      {Object.entries(resources).length > 0 ? (
        moveMode ? (
          <>
            <MoveResources ship={ship} onApplyMove={applyMove} />
            <button className="button red" onClick={() => setMoveMode(false)}>
              Cancel
            </button>
          </>
        ) : (
          <>
            <div>
              <ResourceList resources={resources} />
            </div>
            <button className="button" onClick={() => setMoveMode(true)}>
              Move
            </button>
          </>
        )
      ) : (
        <div className="resource-row">Ship storage is empty</div>
      )}
    </div>
  );
}
