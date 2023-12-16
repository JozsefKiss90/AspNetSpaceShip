import { useCallback, useEffect, useState } from "react";
import { useOutletContext } from "react-router-dom";
import ResourceList from "./ResourceList";
import useHandleFetchError from "../../../hooks/useHandleFetchError";
import { useNotificationsDispatch, getAuthHeaders } from "../../notifications/NotificationContext";

export function ResourceNeeded({ cost, item, onConfirm }) {
  const { station } = useOutletContext();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const [storage, setStorage] = useState(null);
  const [submitting, setSubmitting] = useState(false);

  const fetchStorage = useCallback(async () => {
    try {
      const res = await fetch(`/api/v1/base/${station.id}/storage/resources`,  {
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

  const checkStorage = () => {
    for (const resource of Object.keys(cost)) {
      if (!(resource in storage) || cost[resource] > storage[resource]) {
        return false;
      }
    }
    return true;
  };

  useEffect(() => {
    fetchStorage();
  }, [fetchStorage]);

  async function onClick() {
    setSubmitting(true);
    await onConfirm();
    setSubmitting(false);
  }

  if (storage === null || cost === null) {
    return <div>Loading...</div>;
  }

  return (
    <div className="cost">
      <ResourceList message={`Resources needed to upgrade ${item}:`} resources={cost} />
      {checkStorage() ? (
        <button className="button" onClick={onClick} disabled={submitting}>
          I want it!
        </button>
      ) : (
        <div style={{ color: "red", textShadow: "1px 1px black" }}>Not enough resources</div>
      )}
    </div>
  );
}
