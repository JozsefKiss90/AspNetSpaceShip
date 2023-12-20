import "./Hangar.css";
import { useCallback, useEffect, useState } from "react";
import { useHangarContext } from "../HangarContext";
import { useNavigate } from "react-router-dom";
import useHandleFetchError from "../../../hooks/useHandleFetchError";
import { useNotificationsDispatch, getAuthHeaders } from "../../notifications/NotificationContext";
import LevelDisplay from "./LevelDisplay";

export default function Hangar({ station }) {
  const navigate = useNavigate();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const update = useHangarContext();
  const [hangar, setHangar] = useState(null);

  const fetchHangar = useCallback(async () => {
    try {
      const res = await fetch(`/api/v1/base/${station.id}/hangar`, {
        headers: {
            ...getAuthHeaders(),
            'Content-Type': 'application/json'
        }
    });
      if (res.ok) {
        const data = await res.json();
        setHangar(data);
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
  }, [station, handleFetchError, notifDispatch]);

  useEffect(() => {
    fetchHangar();
  }, [update, fetchHangar]);

  if (hangar === null) {
    return (
      <div className="hangar">
        <div className="menu">
          <LevelDisplay level={"/"} />
          <div>
            <div>HANGAR</div>
            <div>Loading...</div>
          </div>
        </div>
      </div>
    );
  }
  return (
    <div className="hangar">
      <div className="menu">
        <LevelDisplay level={hangar.level} />
        <div>
          <div>HANGAR</div>
          <div>{`${hangar.capacity - hangar.freeDocks}/${hangar.capacity}`}</div>
        </div>
        <button
          className="button push"
          onClick={() => {
            navigate("/station/upgrade/hangar");
          }}
          disabled={hangar.fullyUpgraded}
        >
          {hangar.fullyUpgraded ? "Max level" : "Upgrade"}
        </button>
      </div>
      <ShipList ships={hangar.ships} />
      <div className="add-ship-btn">
        <button
          className="button"
          onClick={() => {
            navigate("ship/add");
          }}
        >
          Add ship
        </button>
      </div>
    </div>
  );
}

function ShipList({ ships }) {
  const navigate = useNavigate();

  if (ships.length === 0) {
    return (
      <div className="ship-list stbar-list">
        <p>No ships yet</p>
      </div>
    );
  } else {
    return (
      <div className="ship-list stbar-list">
        {ships
          .sort((a, b) => a.id - b.id)
          .map((ship) => {
            return (
              <div
                className="shiplist"
                onClick={() => {
                  navigate(`ship/${ship.id}`);
                }}
                key={ship.id}
              >
                {ship.name} - {ship.type}
              </div>
            );
          })}
      </div>
    );
  }
}
