import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import useHandleFetchError from "../../../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../../../notifications/NotificationContext";
import DisplayMission from "./DisplayMission";
import DisplayMiningMission from "./DisplayMiningMission";
import DisplayScoutingMission from "./DisplayScoutingMission";

export default function Mission() {
  const { id } = useParams();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const [mission, setMission] = useState(null);
  const [submitting, setSubmitting] = useState(false);

  const fetchMission = useCallback(async () => {
    try {
      const res = await fetch(`/api/v1/mission/${id}`);
      if (res.ok) {
        const data = await res.json();
        setMission(data);
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
  }, [id, handleFetchError, notifDispatch]);

  useEffect(() => {
    fetchMission();
  }, [fetchMission]);

  async function archiveMission() {
    setSubmitting(true);
    try {
      const res = await fetch(`/api/v1/mission/${id}/archive`, {
        method: "PATCH",
      });
      if (res.ok) {
        const data = await res.json();
        setMission(data);
        notifDispatch({
          type: "add",
          message: "Mission archived.",
          timer: 5,
        });
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
    setSubmitting(false);
  }

  async function abortMission() {
    setSubmitting(true);
    try {
      const res = await fetch(`/api/v1/mission/${id}/abort`, {
        method: "PATCH",
      });
      if (res.ok) {
        const data = await res.json();
        setMission(data);
        notifDispatch({
          type: "add",
          message: "Mission aborted.",
          timer: 5,
        });
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
    setSubmitting(false);
  }

  if (mission === null) {
    return <div>Loading...</div>;
  }

  const missionProps = {
    mission,
    submitting,
    fetchMission,
    abortMission,
    archiveMission,
  };

  if (mission.type === "MINING") {
    return (
      <DisplayMission {...missionProps}>
        <DisplayMiningMission mission={mission} />
      </DisplayMission>
    );
  } else if (mission.type === "SCOUTING") {
    return (
      <DisplayMission {...missionProps}>
        <DisplayScoutingMission mission={mission} />
      </DisplayMission>
    );
  }
}
