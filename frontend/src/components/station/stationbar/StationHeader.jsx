import { useNavigate } from "react-router-dom";
import "./StationHeader.css";

export default function StationHeader({ station }) {
  const navigate = useNavigate();
  return (
    <div className="station-header">
      <div className="station-name">{station.name}</div>
      <div className="station-header-buttons push">
        <div>
          <button className="button" onClick={() => navigate("/station/locations")}>
            Locations
          </button>
        </div>
        <div>
          <button className="button" onClick={() => navigate("/station/missions")}>
            Missions
          </button>
        </div>
      </div>
    </div>
  );
}
