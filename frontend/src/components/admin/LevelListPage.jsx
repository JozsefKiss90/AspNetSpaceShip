import { useNavigate, useOutletContext, useParams } from "react-router-dom";
import "./Admin.css";
import { useCallback, useEffect, useMemo, useState } from "react";
import useHandleFetchError from "../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../notifications/NotificationContext";
import LevelRow from "./LevelRow";
import AddNewLevel from "./AddNewLevel";

export default function LevelListPage() {
  const { user } = useOutletContext();
  const { type } = useParams();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [levels, setLevels] = useState(null);

  const fetchLevels = useCallback(async () => {
    setLoading(true);
    try {
      const res = await fetch(`/api/v1/level?type=${type}`);
      if (res.ok) {
        const data = await res.json();
        setLevels(data.sort((a, b) => a.level - b.level));
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
  }, [type, handleFetchError, notifDispatch]);

  useEffect(() => {
    if (!user) {
      return;
    }
    if (user.role !== "ADMIN") {
      navigate("/");
    } else {
      fetchLevels();
    }
  }, [user, navigate, fetchLevels]);

  function addLevel(newLevel) {
    const newLevels = [...levels];
    const oldMax = { ...newLevels.pop() };
    oldMax.max = false;
    newLevels.push(oldMax);
    newLevels.push(newLevel);
    setLevels(newLevels);
  }

  function updateLevels(newLevel) {
    const newLevels = [...levels].map((level) => {
      if (level.id === newLevel.id) {
        return newLevel;
      } else {
        return level;
      }
    });
    setLevels(newLevels);
  }

  function deleteLastLevel() {
    const newLevels = [...levels];
    newLevels.pop();
    const newMax = { ...newLevels.pop() };
    newMax.max = true;
    newLevels.push(newMax);
    setLevels(newLevels);
  }

  const formatted = useMemo(() => {
    return type.replaceAll("_", " ");
  }, [type]);

  if (loading) {
    return (
      <div className="admin-window">
        <h1>{formatted}</h1>
        <div>Loading...</div>
      </div>
    );
  }

  return (
    <div className="admin-window">
      <h1>{formatted}</h1>
      {levels === null ? (
        <div>Not Available</div>
      ) : (
        <>
          <LevelList
            levels={levels}
            updateLevels={updateLevels}
            deleteLastLevel={deleteLastLevel}
          />
          <AddNewLevel type={type} levelCount={levels.length} addLevel={addLevel}/>
        </>
      )}
    </div>
  );
}

function LevelList({ levels, updateLevels, deleteLastLevel }) {
  if (levels.length === 0) {
    return <div>No levels found.</div>;
  } else {
    return (
      <>
        {levels.map((level) => (
          <LevelRow
            key={level.id}
            level={level}
            updateLevels={updateLevels}
            deleteLastLevel={deleteLastLevel}
          />
        ))}
      </>
    );
  }
}
