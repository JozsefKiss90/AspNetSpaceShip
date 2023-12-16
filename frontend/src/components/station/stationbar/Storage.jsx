import "./Storage.css";
import { useCallback, useEffect, useState } from "react";
import { useStorageContext } from "../StorageContext";
import { useNavigate } from "react-router-dom";
import useHandleFetchError from "../../../hooks/useHandleFetchError";
import { useNotificationsDispatch, getAuthHeaders } from "../../notifications/NotificationContext";
import LevelDisplay from "./LevelDisplay"; 

export default function Storage({ station }) {
  const navigate = useNavigate();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const update = useStorageContext();
  const [storage, setStorage] = useState(null);

  const fetchStorage = useCallback(async () => {
    try {
      const res = await fetch(`/api/v1/base/${station.id}/storage`, {
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
  }, [station, handleFetchError, notifDispatch]);

  useEffect(() => {
    fetchStorage();
  }, [update, fetchStorage]);

  if (storage === null) {
    return (
      <div className="storage">
        <div className="menu">
          <LevelDisplay level={"/"} />
          <div>
            <div>STORAGE</div>
            <div>Loading...</div>
          </div>
        </div>
      </div>
    );
  }
  return (
    <div className="storage">
      <div className="menu">
        <LevelDisplay level={storage.level} />
        <div>
          <div>STORAGE</div>
          <div>{`${storage.capacity - storage.freeSpace}/${storage.capacity}`}</div>
        </div>
        <button
          className="button push"
          onClick={() => navigate("/station/upgrade/storage")}
          disabled={storage.fullyUpgraded}
        >
          {storage.fullyUpgraded ? "Max level" : "Upgrade"}
        </button>
      </div>
      <StorageList resources={storage.resources} />
    </div>
  );
}

function StorageList({ resources }) {
  if (Object.keys(resources).length === 0) {
    return (
      <div className="stbar-list">
        <div className="resource-row">No resources yet</div>
      </div>
    );
  } else {
    const order = ["METAL", "CRYSTAL", "SILICONE", "PLUTONIUM"].filter(
      (resource) => resource in resources
    );
    return (
      <div className="stbar-list">
        {order.map((key) => {
          return (
            <div className="resource-row" key={key}>
              <img src={"/" + key.toLowerCase() + ".png"} alt={key} />
              <div>
                {key}: {resources[key]}
              </div>
            </div>
          );
        })}
      </div>
    );
  }
}
