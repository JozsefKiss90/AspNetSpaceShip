import { useCallback, useEffect, useState } from "react";
import { useNavigate, useOutletContext } from "react-router-dom";
import { useStorageDispatchContext } from "../../StorageContext";
import { useHangarDispatchContext } from "../../HangarContext";
import "./AddShip.css";
import ResourceList from "../ResourceList";
import useHandleFetchError from "../../../../hooks/useHandleFetchError";
import { useNotificationsDispatch, getAuthHeaders } from "../../../notifications/NotificationContext";

export default function AddShip() {
  const { station } = useOutletContext();
  const navigate = useNavigate();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const storageSetter = useStorageDispatchContext();
  const hangarSetter = useHangarDispatchContext();
  const [shipType, setShipType] = useState("miner");
  const [cost, setCost] = useState(null);
  const [colors, setColors] = useState(null);
  const [storage, setStorage] = useState(null);
  const [hasAvailableDock, setHasAvailableDock] = useState(null);
  const [loading, setLoading] = useState(true);
  console.log(station)
  const getShipCost = useCallback(async () => {
    setLoading(true);
    try {
      const res = await fetch(`/api/v1/ship/cost/${shipType}`, {
        headers: {
            ...getAuthHeaders(),
            'Content-Type': 'application/json'
        }
    });
      if (res.ok) {
        const data = await res.json();
        setCost(data);
        setLoading(false);
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      notifDispatch({
        type: "generic error",
      });
    }
  }, [shipType, handleFetchError, notifDispatch]);

  const getStationData = useCallback(async () => {
    if (colors === null || storage === null || hasAvailableDock === null) {
      try {
        const urls = [
          `/api/v1/ship/color`,
          `/api/v1/base/${station.id}/storage/resources`,
          `/api/v1/base/${station.id}/hangar`,
        ];
        const [colors, resources, hangar] = await Promise.all(
          urls.map(async (url) => {
            const res = await fetch(url, {
              headers: {
                  ...getAuthHeaders(),
                  'Content-Type': 'application/json'
              }
          });
            if (res.ok) {
              return res.json();
            } else {
              handleFetchError(res);
            }
          })
        );
        console.log(hangar)
        setColors(colors);
        setStorage(resources);
        setHasAvailableDock(hangar.freeDocks > 0);
      } catch (err) {
        console.error(err);
        notifDispatch({
          type: "generic error",
        });
      }
    }
  }, [colors, storage, hasAvailableDock, station, handleFetchError, notifDispatch]);
  console.log(storage)
  useEffect(() => {
    getStationData();
  }, [getStationData]);

  useEffect(() => {
    getShipCost();
  }, [getShipCost]);

  function checkStorage() {
    console.log(cost)
    for (const resource of Object.keys(cost)) {
      if (!(resource in storage) || cost[resource] > storage[resource]) {
        return false;
      }
    }
    return true;
  }

  function onSubmit(e) {
    e.preventDefault();
    setLoading(true);
    const formData = new FormData(e.target);
    const entries = [...formData.entries()];

    const shipData = entries.reduce((acc, entry) => {
      const [k, v] = entry;
      acc[k] = v;
      return acc;
    }, {});
    addShip(shipData);
  }

  async function addShip(shipData) {
    try {
      const res = await fetch(`/api/v1/base/${station.id}/add/ship`, {
        method: "POST",
        headers: {
          ...getAuthHeaders(),
          "Content-Type": "application/json",
        },
        body: JSON.stringify(shipData),
      });
      if (res.ok) {
        const data = await res.json();
        storageSetter({ type: "update" });
        hangarSetter({ type: "update" });
        notifDispatch({
          type: "add",
          message: "New ship built.",
          timer: 5,
        });
        navigate(`/station/ship/${data}`);
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
  }

  function AddButton() {
    if (!hasAvailableDock) {
      return (
        <div style={{ color: "red", textShadow: "1px 1px black" }}>No available dock in hangar</div>
      );
    } else if (!checkStorage()) {
      return <div style={{ color: "red", textShadow: "1px 1px black" }}>Not enough resources</div>;
    } else {
      return (
        <div>
          <button className="button">Buy ship</button>
        </div>
      );
    }
  }

  if (colors === null || storage === null || hasAvailableDock === null) {
    return <div>Loading...</div>;
  }

  return (
    <div className="add-ship">
      <h2>Buy new ship:</h2>
      <form onSubmit={onSubmit}>
        <div>
          <label htmlFor="name">Name: </label>
          <input
            name="name"
            id="name"
            type="text"
            placeholder="Enter ship name"
            size="15"
            minLength="3"
            maxLength="15"
            required
          ></input>
        </div>
        <div>
          <label htmlFor="type">Type: </label>
          <select name="type" id="type" onChange={(e) => setShipType(e.target.value)}>
            <option value="MINER">Miner</option>
            <option value="SCOUT">Scout</option>
          </select>
        </div>
        <div>
          <label htmlFor="color">Color: </label>
          <select name="color" id="color">
            {colors.map((color) => (
              <option key={color}>{color}</option>
            ))}
          </select>
        </div>
        {!loading ? ( 
          <>
            <ResourceList message={"Resources needed:"} resources={cost} />
            <AddButton />
          </>
        ) : (
          <div>Loading...</div>
        )}
      </form>
    </div>
  );
}
