import { useCallback, useEffect, useState } from "react";
import { useNavigate, useOutletContext } from "react-router-dom";
import "./LocationList.css";
import Location from "./Location";
import useHandleFetchError from "../../../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../../../notifications/NotificationContext";

export default function LocationListPage() {
  const { station } = useOutletContext();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const navigate = useNavigate();
  const [locationsLoading, setLocationsLoading] = useState(false);
  const [locations, setLocations] = useState(null);
  const [availableShips, setAvailableShips] = useState(null);
  const [showDepleted, setShowDepleted] = useState(false);
  const [resourceFilter, setResourceFilter] = useState("ALL");
  const [sort, setSort] = useState("discoveredDESC");

  const fetchLocations = useCallback(
    async (signal) => {
      setLocations(null);
      setLocationsLoading(true);
      try {
        const res = await fetch(
          `/api/v1/location?includeDepleted=${showDepleted}${
            resourceFilter === "ALL" ? "" : `&resources=${resourceFilter}`
          }&sort=${sort}`,
          { signal }
        );
        if (res.ok) {
          const data = await res.json();
          setLocations(data);
        } else {
          handleFetchError(res);
        }
        setLocationsLoading(false);
      } catch (err) {
        if (err.name !== "AbortError") {
          console.error(err);
          notifDispatch({
            type: "generic error",
          });
          setLocationsLoading(false);
        }
      }
    },
    [showDepleted, resourceFilter, sort, handleFetchError, notifDispatch]
  );

  const fetchShips = useCallback(async () => {
    try {
      const res = await fetch(`/api/v1/base/${station.id}/hangar`);
      if (res.ok) {
        const data = await res.json();
        setAvailableShips(
          data.ships
            .filter((ship) => ship.missionId === 0 && ship.type === "MINER")
            .sort((a, b) => a.id - b.id)
        );
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
    fetchShips();
  }, [fetchShips]);

  useEffect(() => {
    const controller = new AbortController();
    const signal = controller.signal;
    fetchLocations(signal);
    return () => {
      controller.abort();
    };
  }, [fetchLocations]);

  const loaded = !locationsLoading && availableShips !== null;

  return (
    <div className="location-list">
      <div className="location-list-top">
        <div className="location-list-title">Available locations:</div>
        <button className="button" onClick={() => navigate("/station/locations/explore")}>
          Explore
        </button>
      </div>
      <div className="location-list-top">
        <div>
          <label htmlFor="filter">Filter resource: </label>
          <select
            name="filter"
            id="filter"
            value={resourceFilter}
            onChange={(e) => setResourceFilter(e.target.value)}
          >
            <option value="ALL">All</option>
            <option value="METAL">Metal</option>
            <option value="CRYSTAL">Crystal</option>
            <option value="SILICONE">Silicone</option>
            <option value="PLUTONIUM">Plutonium</option>
          </select>
        </div>
        <div>
          <label htmlFor="depleted">Show depleted:</label>
          <input type="checkbox" onClick={() => setShowDepleted(!showDepleted)} />
        </div>
        <div>
          <label htmlFor="orderBy">Order by: </label>
          <select
            name="orderBy"
            id="orderBy"
            value={sort}
            onChange={(e) => setSort(e.target.value)}
          >
            <option value="discoveredDESC">Discovery (newest first)</option>
            <option value="discoveredASC">Discovery (oldest first)</option>
            <option value="nameASC">Name (A-Z)</option>
            <option value="nameDESC">Name (Z-A)</option>
            <option value="reserveASC">Resources (ascending)</option>
            <option value="reserveDESC">Resources (descending)</option>
            <option value="distanceASC">Distance (ascending)</option>
            <option value="distanceDESC">Distance (descending)</option>
          </select>
        </div>
      </div>
      <LocationList loaded={loaded} locations={locations} availableShips={availableShips} />
    </div>
  );
}

function LocationList({ loaded, locations, availableShips }) {
  if (!loaded) {
    return <div>Loading...</div>;
  } else if (locations === null || locations.length === 0) {
    return <div>No locations to show.</div>;
  } else {
    return (
      <>
        {locations.map((location) => (
          <Location key={location.id} location={location} availableShips={availableShips} />
        ))}
      </>
    );
  }
}
