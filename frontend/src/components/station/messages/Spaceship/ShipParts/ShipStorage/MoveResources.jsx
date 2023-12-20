import { useCallback, useEffect, useState } from "react";
import { useOutletContext } from "react-router";
import ResourceRow from "./ResourceRow";
import useHandleFetchError from "../../../../../../hooks/useHandleFetchError";
import { useNotificationsDispatch, getAuthHeaders } from "../../../../../notifications/NotificationContext";

export default function MoveResources({ ship, onApplyMove }) {
  const { station } = useOutletContext();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const [storage, setStorage] = useState(null);
  const [moveAmount, setMoveAmount] = useState({});
  const [loading, setLoading] = useState(true);

  const fetchStorage = useCallback(async () => {
    setLoading(true);
    try {
      const res = await fetch(`/api/v1/base/${station.id}/storage`,  {
        headers: {
            ...getAuthHeaders(),
            'Content-Type': 'application/json'
        }
    });
      if (res.ok) {
        const data = await res.json();
        setStorage(data);
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
    setLoading(false);
  }, [station, handleFetchError, notifDispatch]);

  useEffect(() => {
    fetchStorage();
  }, [fetchStorage]);

  const confirmMove = useCallback(async () => {
    setLoading(true);
    try {
      const res = await fetch(`/api/v1/base/${station.id}/add/resource-from-ship?ship=${ship.id}`, {
        method: "PATCH",
        headers: {
          ...getAuthHeaders(),
          "Content-Type": "application/json",
        },
        body: JSON.stringify(moveAmount),
      });
      if (res.ok) {
        const data = await res.json();
        if (data === true) {
          onApplyMove(moveAmount);
          notifDispatch({
            type: "add",
            message: "Resources moved.",
            timer: 5,
          });
        }
      } else {
        handleFetchError(res);
      }
    } catch (error) {
      console.error(error);
      notifDispatch({
        type: "generic error",
      });
    }
    setLoading(false);
  }, [station, ship, moveAmount, onApplyMove, handleFetchError, notifDispatch]);

  function updateMoveAmount(resource, amount) {
    const newMoveAmount = { ...moveAmount };
    newMoveAmount[resource] = amount;
    setMoveAmount(newMoveAmount);
  }

  if (loading) {
    return <div>Loading...</div>;
  }

  const totalMoved = Object.values(moveAmount).reduce((sum, next) => sum + next, 0);
  const freeSpace = storage.freeSpace - totalMoved;

  return (
    <div className="move-resources">
      <table>
        <tbody>
          <tr>
            <td colSpan={2}></td>
            <td>Ship</td>
            <td>{"->"}</td>
            <td>Station</td>
          </tr>
          {Object.keys(ship.storage.resources).map((resource) => {
            if (ship.storage.resources[resource] > 0) {
              return (
                <ResourceRow
                  key={resource}
                  resource={resource}
                  shipResource={ship.storage.resources[resource]}
                  movedResource={moveAmount[resource] ? moveAmount[resource] : 0}
                  stationResource={storage.resources[resource] ? storage.resources[resource] : 0}
                  onChange={updateMoveAmount}
                />
              );
            } else {
              return null;
            }
          })}
        </tbody>
      </table>
      <div style={freeSpace >= 0 ? {} : { color: "red", textShadow: "1px 1px black" }}>
        Free space: {freeSpace}
      </div>
      {freeSpace >= 0 && (
        <div>
          <button className="button" onClick={confirmMove}>
            Move
          </button>
        </div>
      )}
    </div>
  );
}
