import { useCallback, useEffect, useState } from "react";
import "./MissionList.css";
import { useNavigate } from "react-router-dom";
import useHandleFetchError from "../../../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../../../notifications/NotificationContext";

export default function MissionList() {
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const [missions, setMissions] = useState(null);
  const [listToShow, setListToShow] = useState("active");
  const [loading, setLoading] = useState(true);

  const fetchMissions = useCallback(
    async (signal) => {
      setLoading(true);
      try {
        const res = await fetch(`/api/v1/mission/${listToShow}`, { signal });
        if (res.ok) {
          const data = await res.json();
          setMissions(data);
        } else {
          handleFetchError(res);
        }
        setLoading(false);
      } catch (err) {
        if (err.name !== "AbortError") {
          console.error(err);
          notifDispatch({
            type: "generic error",
          });
          setLoading(false);
        }
      }
    },
    [listToShow, handleFetchError, notifDispatch]
  );

  useEffect(() => {
    const controller = new AbortController();
    const signal = controller.signal;
    fetchMissions(signal);
    return () => {
      controller.abort();
    };
  }, [fetchMissions]);

  return (
    <div className="mission-list">
      <div className="ml-header">
        <div>Missions:</div>
        <div>
          <button
            className="ml-list-toggle"
            onClick={() => {
              setMissions(null);
              setListToShow("active");
            }}
            disabled={listToShow === "active"}
          >
            Active
          </button>
          <span> | </span>
          <button
            className="ml-list-toggle"
            onClick={() => {
              setMissions(null);
              setListToShow("archived");
            }}
            disabled={listToShow === "archived"}
          >
            Archived
          </button>
        </div>
      </div>
      <MissionListElement />
    </div>
  );

  function MissionListElement() {
    if (loading) {
      return <div>Loading...</div>;
    } else if (missions === null || missions.length === 0) {
      return <div>No missions to show.</div>;
    } else {
      return (
        <>
          {missions.map((mission) => {
            return mission.status === "ARCHIVED" ? (
              <ArchivedMissionListItem key={mission.id} mission={mission} />
            ) : (
              <ActiveMissionListItem key={mission.id} mission={mission} />
            );
          })}
        </>
      );
    }
  }
}

function ActiveMissionListItem({ mission }) {
  const navigate = useNavigate();
  const formattedStatus = mission.status.replaceAll("_", " ");
  const nextReport = new Date(mission.currentObjectiveTime + "Z");
  return (
    <div className="ml-li ml-li-active" onClick={() => navigate(`/station/mission/${mission.id}`)}>
      <div>
        <div>{mission.title}</div>
        {mission.status !== "OVER" && <div>Next report : {nextReport.toLocaleString("hu-HU")}</div>}
      </div>
      <div>{formattedStatus}</div>
    </div>
  );
}

function ArchivedMissionListItem({ mission }) {
  const navigate = useNavigate();
  const endTime = new Date(mission.approxEndTime + "Z");

  return (
    <div className="ml-li" onClick={() => navigate(`/station/mission/${mission.id}`)}>
      <div>{mission.title}</div>
      <div>Ended: {endTime.toLocaleString("hu-HU")}</div>
    </div>
  );
}
