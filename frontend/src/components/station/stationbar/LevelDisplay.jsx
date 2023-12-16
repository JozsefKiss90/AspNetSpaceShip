import "./LevelDisplay.css";

export default function LevelDisplay({ level }) {
  return (
    <div className="lvl-display">
      <div>lvl</div>
      <div>{level}</div>
    </div>
  );
}
