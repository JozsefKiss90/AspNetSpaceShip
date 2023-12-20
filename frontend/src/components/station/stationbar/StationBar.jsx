import "./StationBar.css";
import Storage from "./Storage";
import Hangar from "./Hangar";
import StationHeader from "./StationHeader";

const StationBar = ({ station }) => {
  return (
    <div className="station-bar">
      <StationHeader station={station} />
      <Storage station={station} />
      <Hangar station={station} />
    </div>
  );
};

export default StationBar;
