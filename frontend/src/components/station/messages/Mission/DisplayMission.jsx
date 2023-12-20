import { useRef } from "react";
import Countdown from "react-countdown";
import MissionReports from "./MissionReports";
import "./Mission.css";

export default function DisplayMission({
  children,
  mission,
  submitting,
  fetchMission,
  abortMission,
  archiveMission,
}) {
  const timer = useRef();

  async function onComplete() {
    await fetchMission();
    timer.current.getApi().start();
  }

  const active = mission.status !== "OVER" && mission.status !== "ARCHIVED";
  const endTime = new Date(mission.approxEndTime + "Z");
  const nextReport = new Date(mission.currentObjectiveTime + "Z");

  return (
    <>
      <div className="mission-container">
        <div className="mission-title">{mission.title}</div>
        {children}
        {active && (
          <div className="mission-timer">
            <div>Next expected report</div>
            <Countdown
              ref={timer}
              className="mission-countdown"
              date={nextReport}
              onComplete={onComplete}
              daysInHours={true}
            />
          </div>
        )}
        <div className="mission-end">
          <div>{active ? "Expected mission completion" : "Mission ended"}</div>
          <div>{endTime.toLocaleString("hu-HU")}</div>
        </div>
        <MissionReports reports={mission.reports} />
        <div className="mission-actions">
          {active && mission.status !== "RETURNING" && (
            <button className="button red" onClick={abortMission} disabled={submitting}>
              Abort
            </button>
          )}
          {mission.status === "OVER" && (
            <button className="button" onClick={archiveMission} disabled={submitting}>
              Archive
            </button>
          )}
        </div>
      </div>
    </>
  );
}
