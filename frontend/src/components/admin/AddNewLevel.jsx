import { useState } from "react";
import EditLevelRow from "./EditLevelRow";
import useHandleFetchError from "../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../notifications/NotificationContext";

export default function AddNewLevel({ type, levelCount, addLevel }) {
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const [add, setAdd] = useState(false);

  async function onAddConfirm(effect, cost) {
    try {
      const res = await fetch(`/api/v1/level`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          type,
          effect,
          cost,
        }),
      });
      if (res.ok) {
        const data = await res.json();
        addLevel(data);
        setAdd(false);
        notifDispatch({
          type: "add",
          message: "Level added.",
          notifType: "admin",
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
  }

  if (add) {
    return (
      <div className="admin-list-elem">
        <EditLevelRow
          level={{
            level: levelCount + 1,
            effect: 0,
            cost: {},
          }}
          onConfirm={onAddConfirm}
          confirmText="Add level"
          onCancel={() => setAdd(false)}
        />
      </div>
    );
  }
  return (
    <div
      className="admin-list-elem list-clickable"
      onClick={() => setAdd(true)}
    >
      <div>+ Add new level</div>
    </div>
  );
}
