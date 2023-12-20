import StationBar from "./stationbar/StationBar";
import "./Station.css";
import "./messages/Messages.css";
import { StorageStationContext } from "./StorageContext";
import { HangarStationContext } from "./HangarContext";
import { Outlet, useNavigate, useOutletContext } from "react-router-dom";
import { useCallback, useEffect, useState } from "react";
import { useNotificationsDispatch, getAuthHeaders } from "../notifications/NotificationContext";

const Station = () => {
  const context = useOutletContext();
  const { user } = context;
  const notifDispatch = useNotificationsDispatch();
  const navigate = useNavigate();
  const [station, setStation] = useState(null);


  const fetchBaseId = useCallback(async () => {
    try {
      const res = await fetch('/api/v1/base/', {
        headers: {
            ...getAuthHeaders(),
            'Content-Type': 'application/json'
        }
    });
      if (res.ok) {
        const data = await res.json();
        setStation(data);
      } else {
        notifDispatch({
          type: "add",
          message: "Couldn't get station for user",
          notifType: "error",
          timer: 0, 
        });
        navigate("/");
      }
    } catch (err) {
      console.error(err);
      navigate("/error");
    }
  }, [user]);

  useEffect(() => {
    fetchBaseId();
  }, [fetchBaseId]);

  if (station === null) {
    return <div>Loading...</div>;
  }
  return (
    <>
      <div className="station">
        <StorageStationContext>
          <HangarStationContext>
            <StationBar station={station} />
            <div className="messages">
              <Outlet context={{ ...context, station }} />
            </div>
          </HangarStationContext>
        </StorageStationContext>
      </div>
    </>
  );
};

export default Station;
