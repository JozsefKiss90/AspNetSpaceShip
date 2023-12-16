import { useState } from "react";
import "./Location.css";
import { useNavigate } from "react-router-dom";
import useHandleFetchError from "../../../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../../../notifications/NotificationContext";

export default function Location({ location, availableShips }) {
  const navigate = useNavigate();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const [missionMenuToggle, setMissionMenuToggle] = useState(false);
  const [submitting, setSubmitting] = useState(false);

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
    details.locationId = location.id;
    startMission(details);
  }

  function onCancel() {
    setMissionMenuToggle(false);
  }

  async function startMission(details) {
    try {
      const res = await fetch("/api/v1/mission/miner", {
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

  const formattedDate = new Date(location.discovered + "Z").toLocaleString("hu-HU");
  return (
    <div className="location">
      <div className="loc-details">
        <img src="/planet.png" alt="planet" style={{ width: "100px", height: "100px" }} />
        <div className="loc-data">
          <div className="loc-top">
            <div className="loc-name">{location.name}</div>
            {!missionMenuToggle && (
              <MenuButton location={location} setMissionMenuToggle={setMissionMenuToggle} />
            )}
          </div>
          <div className="loc-secondary-data">
            <div>
              <div className="loc-data-label">Resource:</div>
              <div>{location.resourceType}</div>
            </div>
            <div>
              <div className="loc-data-label">Reserves:</div> <div>{location.resourceReserve}</div>
            </div>
            <div>
              <div className="loc-data-label">Distance:</div>{" "}
              <div>{location.distanceFromStation} AUs</div>
            </div>
            <div>
              <div className="loc-data-label">Discovered:</div> <div>{formattedDate}</div>
            </div>
          </div>
        </div>
      </div>
      {missionMenuToggle && (
        <MissionMenu
          availableShips={availableShips}
          submitting={submitting}
          onSubmitMission={onSubmitMission}
          onCancel={onCancel}
        />
      )}
    </div>
  );
}

function MenuButton({ location, setMissionMenuToggle }) {
  const navigate = useNavigate();
  if (location.missionId === 0) {
    return (
      <div>
        <button className="button" onClick={() => setMissionMenuToggle(true)}>
          Start mission
        </button>
      </div>
    );
  } else {
    return (
      <div>
        <button
          className="button"
          onClick={() => navigate(`/station/mission/${location.missionId}`)}
        >
          Check mission
        </button>
      </div>
    );
  }
}

function MissionMenu({ availableShips, submitting, onSubmitMission, onCancel }) {
  return (
    <form className="loc-mission-menu" onSubmit={onSubmitMission}>
      <div>
        <label htmlFor="shipId">Ship: </label>
        <select name="shipId" required>
          {availableShips.length > 0 ? (
            availableShips.map((ship) => (
              <option key={ship.id} value={ship.id}>
                {ship.name}
              </option>
            ))
          ) : (
            <option disabled>No available miner ships</option>
          )}
        </select>
      </div>
      <div>
        <label htmlFor="activityTime">Mine for: </label>
        <select name="activityTime" required>
          <option value={3600}>1 hour</option>
          <option value={7200}>2 hours</option>
          <option value={10800}>3 hours</option>
          <option value={14400}>4 hours</option>
          <option value={60}>DEMO</option>
        </select>
      </div>
      <div className="loc-mission-actions">
        <button className="button" type="submit" disabled={submitting}>
          Start
        </button>
        <button className="button red" onClick={onCancel}>
          Cancel
        </button>
      </div>
    </form>
  );
}
