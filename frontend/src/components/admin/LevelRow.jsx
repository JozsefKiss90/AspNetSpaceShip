import { useState, useMemo } from "react";
import useHandleFetchError from "../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../notifications/NotificationContext";
import EditLevelRow from "./EditLevelRow";

export default function LevelRow({ level, updateLevels, deleteLastLevel }) {
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const [edit, setEdit] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  async function onEditConfirm(effect, cost) {
    if (!window.confirm("Changing levels can lead to errors. Are you sure?")) {
      return;
    }
    try {
      const res = await fetch(`/api/v1/level/${level.id}`, {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          type: level.type,
          effect,
          cost,
        }),
      });
      if (res.ok) {
        const data = await res.json();
        updateLevels(data);
        setEdit(false);
        notifDispatch({
          type: "add",
          message: "Level updated.",
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

  async function onDeleteLevel() {
    if (!window.confirm("Deleting levels can lead to errors. Are you sure?")) {
      setSubmitting(false);
      return;
    }
    setSubmitting(true);
    try {
      const res = await fetch(`/api/v1/level?type=${level.type}`, {
        method: "DELETE",
      });
      if (res.ok) {
        const data = await res.json();
        if (data === true) {
          deleteLastLevel();
          notifDispatch({
            type: "add",
            message: "Level deleted.",
            notifType: "admin",
            timer: 5,
          });
        }
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

  if (edit) {
    return (
      <div className="admin-list-elem">
        <EditLevelRow
          level={level}
          onConfirm={onEditConfirm}
          confirmText="Save changes"
          onCancel={() => setEdit(false)}
          cancelText="Cancel edit"
        />
      </div>
    );
  }

  return (
    <div className="admin-list-elem">
      <div className="admin-level">
        <div>
          <div>Level:</div>
          <div>{level.level}</div>
        </div>
        <div>
          <div>Effect:</div>
          <div>{level.effect}</div>
        </div>
        <div>
          <div>Cost:</div>
          <Cost cost={level.cost} />
        </div>
        <div className="admin-level-actions">
          <>
            <button className="button blue" onClick={() => setEdit(true)}>
              Edit
            </button>
            {level.max && level.level > 1 && (
              <button
                className="button red"
                onClick={onDeleteLevel}
                disabled={submitting}
              >
                Delete
              </button>
            )}
          </>
        </div>
      </div>
    </div>
  );
}

function Cost({ cost }) {
  const costList = useMemo(() => {
    return Object.entries(cost);
  }, [cost]);
  if (costList.length === 0) {
    return <div>No cost</div>;
  } else {
    return (
      <div>
        {costList.map((entry) => (
          <div className="admin-level-cost" key={entry[0]}>
            <div>
            {entry[0]}:
            </div>
            <div>
            {entry[1]}
            </div>
          </div>
        ))}
      </div>
    );
  }
}
