import { useNavigate, useOutletContext, useParams } from "react-router-dom";
import { ResourceNeeded } from "./ResourceNeeded";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useStorageDispatchContext } from "../StorageContext";
import { useHangarDispatchContext } from "../HangarContext";
import useHandleFetchError from "../../../hooks/useHandleFetchError";
import { useNotificationsDispatch, getAuthHeaders } from "../../notifications/NotificationContext";

export default function StationUpgrade() {
  const { stationModule } = useParams();
  const { station } = useOutletContext();
  const navigate = useNavigate();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const storageSetter = useStorageDispatchContext();
  const hangarSetter = useHangarDispatchContext();
  const modules = useMemo(() => ["hangar", "storage"], []);
  const [cost, setCost] = useState(null);
  const [loading, setLoading] = useState(true);

  const fetchCost = useCallback(
    async (signal) => {
      if (!modules.includes(stationModule)) {
        navigate("/404");
      }
      setCost(null);
      setLoading(true);
      try {
        const res = await fetch(`/api/v1/base/${station.id}/${stationModule}/upgrade`, {
          headers: {
              ...getAuthHeaders(),
              'Content-Type': 'application/json'
          }
      }, { signal });
        if (res.ok) {
          const data = await res.json();
          setCost(data);
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
    [modules, stationModule, navigate, station, handleFetchError, notifDispatch]
  );

  useEffect(() => {
    const controller = new AbortController();
    const signal = controller.signal;
    fetchCost(signal);
    return () => controller.abort();
  }, [fetchCost]);

  if (!modules.includes(stationModule)) {
    return <div>404</div>;
  }

  async function onConfirm() {
    try {
      const res = await fetch(`/api/v1/base/${station.id}/${stationModule}/upgrade`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({}),
      });
      if (res.ok) {
        storageSetter({ type: "update" });
        if (stationModule === "hangar") {
          hangarSetter({ type: "update" });
        }
        notifDispatch({
          type: "add",
          message: `${stationModule.toUpperCase()} upgraded.`,
          timer: 5,
        });
        navigate("/station");
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
  }

  if (loading) {
    return <div>Loading...</div>;
  }
  if (cost === null) {
    return <div>Not available</div>;
  }
  return <ResourceNeeded cost={cost} item={stationModule} onConfirm={onConfirm} />;
}
