export default function ResourceRow({
  resource,
  shipResource,
  movedResource,
  stationResource,
  onChange,
}) {
  function filterInput(e) {
    const input = Math.max(0, Math.min(shipResource, parseInt(e.target.value)));
    return (Number.isNaN(input)) ? 0 : input;
  }
  return (
    <tr key={resource}>
      <td>
        <img
          style={{ width: "25px", height: "25px" }}
          src={"/" + resource.toLowerCase() + ".png"}
          alt={resource}
        />
      </td>
      <td>{resource}</td>
      <td>{shipResource - movedResource}</td>
      <td>
        <input
          type="number"
          min={0}
          max={shipResource}
          value={movedResource}
          onChange={(e) => onChange(resource, filterInput(e))}
        />
      </td>
      <td>{stationResource + movedResource}</td>
    </tr>
  );
}
