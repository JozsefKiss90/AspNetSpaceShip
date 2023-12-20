export default function MissionReports({ reports }) {
  return (
    <div className="mission-reports">
      <div>Reports:</div>
      {reports.map((report) => (
        <Report key={report.message} report={report} />
      ))}
    </div>
  );
}

function Report({ report }) {
  const formattedDate = `<${new Date(report.time + "Z").toLocaleString("hu-HU")}>`;
  return (
    <div className="mission-report">
      <div>{formattedDate}</div>
      <div>
        {report.message.split("\n").map((line) => (
          <div key={line}>{line}</div>
        ))}
      </div>
    </div>
  );
}
