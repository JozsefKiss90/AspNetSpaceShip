import { useNavigate, useOutletContext } from "react-router-dom";
import "./Admin.css";
import { useCallback, useEffect, useState } from "react";
import useHandleFetchError from "../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../notifications/NotificationContext";

export default function LevelTypes() {
  const { user } = useOutletContext();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [types, setTypes] = useState(null);

  const fetchTypes = useCallback(async () => {
    setLoading(true);
    try {
      const res = await fetch("/api/v1/level/types");
      if (res.ok) {
        const data = await res.json();
        setTypes(data);
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
  }, [handleFetchError, notifDispatch]);

  console.log(types);

  useEffect(() => {
    if (!user) {
      return;
    }
    if (user.role !== "ADMIN") {
      navigate("/");
    } else {
      fetchTypes();
    }
  }, [user, navigate, fetchTypes]);

  if (loading) {
    return (
      <div className="admin-window">
        <h1>Level categories</h1>
        <div>Loading...</div>
      </div>
    );
  }

  return (
    <div className="admin-window">
      <h1>Level categories</h1>
      {types === null ? <div>Not Available</div> : <TypeList types={types} />}
    </div>
  );
}

function TypeList({ types }) {
  if (types.length === 0) {
    return <div>No categories found.</div>;
  } else {
    return (
      <>
        {types.map((type) => (
          <Type key={type} type={type} />
        ))}
      </>
    );
  }
}

function Type({ type }) {
  const navigate = useNavigate();
  const formatted = type.replaceAll("_", " ");
  return (
    <div
      className="admin-list-elem list-clickable"
      onClick={() => navigate(`/admin/levels/${type}`)}
    >
      <div>{formatted}</div>
    </div>
  );
}
