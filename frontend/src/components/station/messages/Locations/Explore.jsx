import { useEffect, useState } from "react";
import { useCallback } from "react";
import { useNavigate, useOutletContext } from "react-router-dom";
import { useNotificationsDispatch } from "../../../notifications/NotificationContext";
import useHandleFetchError from "../../../../hooks/useHandleFetchError";
import "./Explore.css";

export default function Explore() {
  const { station } = useOutletContext();
  const notifDispatch = useNotificationsDispatch();
  const handleFetchError = useHandleFetchError();
  const navigate = useNavigate();
  const [availableShips, setAvailableShips] = useState(null);
  const [submitting, setSubmitting] = useState(false);

  const fetchScoutShips = useCallback(async () => {
    try {
      const res = await fetch(`/api/v1/base/${station.id}/hangar`);
      if (res.ok) {
        const data = await res.json();
        setAvailableShips(
          data.ships
            .filter((ship) => ship.missionId === 0 && ship.type === "SCOUT")
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
  }, [station.id, handleFetchError, notifDispatch]);

  useEffect(() => {
    fetchScoutShips();
  }, [fetchScoutShips]);

  function onSubmitMission(e) {
    e.preventDefault();
    setSubmitting(true);
    const formData = new FormData(e.target);
    const entries = [...formData.entries()];
    const details = entries.reduce((acc, entry) => {
      const [k, v] = entry;
      acc[k] = v;
      return acc;
    }, {});
    details.activityTime = details.activityTime * 60 * 60;
    startMission(details);
  }

  async function startMission(details) {
    try {
      const res = await fetch("/api/v1/mission/scout", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(details),
      });
      if (res.ok) {
        const data = await res.json();
        navigate(`/station/mission/${data.id}`);
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      notifDispatch({
        type: "generic error",
      });
    }
    setSubmitting(false);
  }

  const loading = availableShips === null;

  return (
    <div className="explore">
      <h1>Explore new locations</h1>
      <form onSubmit={onSubmitMission}>
        <ShipSelect availableShips={availableShips} />
        <DistanceSelect />
        <PrioritySelect />
        <ResourceSelect />
        <DurationSelect />
        <div className="exp-actions">
          <button className="button" type="submit" disabled={loading || submitting}>
            Start mission
          </button>
          <button className="button red" onClick={() => navigate("/station/locations")}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

function ShipSelect({ availableShips }) {
  if (availableShips === null) {
    return (
      <div className="exp-form exp-ships">
        <div>
          <label htmlFor="shipId">Ship: </label>
          <span>Loading...</span>
        </div>
      </div>
    );
  } else {
    return (
      <div className="exp-form exp-ships">
        <div>
          <label htmlFor="shipId">Ship: </label>
          <select name="shipId" id="shipId" required>
            {availableShips.length > 0 ? (
              availableShips.map((ship) => (
                <option key={ship.id} value={ship.id}>
                  {ship.name}
                </option>
              ))
            ) : (
              <option disabled>No available scout ships</option>
            )}
          </select>
        </div>
      </div>
    );
  }
}

function DistanceSelect() {
  return (
    <div className="exp-form exp-distance">
      <div>
        <label htmlFor="distance">Distance (AUs): </label>
        <input
          name="distance"
          id="distance"
          type="number"
          min={1}
          max={50}
          defaultValue={1}
          required
        />
      </div>
    </div>
  );
}

function ResourceSelect() {
  return (
    <div className="exp-form exp-resoruce">
      <div>
        <label htmlFor="targetResource">Resource: </label>
        <select name="targetResource" id="targetResource" required>
          <option value="METAL">Metal</option>
          <option value="CRYSTAL">Crystal</option>
          <option value="SILICONE">Silicone</option>
          <option value="PLUTONIUM">Plutonioum</option>
        </select>
      </div>
    </div>
  );
}

function DurationSelect() {
  return (
    <div className="exp-form exp-duration">
      <div>
        <label htmlFor="activityTime">Scan duration (hours): </label>
        <input
          name="activityTime"
          id="activityTime"
          type="number"
          min={1}
          max={10}
          defaultValue={1}
          required
        />
      </div>
    </div>
  );
}

function PrioritySelect() {
  return (
    <div className="exp-form exp-priority">
      <div>
        <div>Priority:</div>
        <div>
          <div>
            <input
              type="radio"
              name="prioritizingDistance"
              id="close"
              value={true}
              defaultChecked
            />
            <label htmlFor="close">Closeness</label>
          </div>
          <div>
            <input type="radio" name="prioritizingDistance" id="rich" value={false} />
            <label htmlFor="rich">Resource abundance</label>
          </div>
        </div>
      </div>
    </div>
  );
}
