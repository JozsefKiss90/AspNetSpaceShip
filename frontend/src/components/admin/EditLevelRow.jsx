import { useState } from "react";

export default function EditLevelRow({ level, onConfirm, confirmText, onCancel, cancelText }) {
  const [effect, setEffect] = useState(level.effect);
  const [cost, setCost] = useState(level.cost);
  const [submitting, setSubmitting] = useState(false);

  confirmText = confirmText ?? "Confirm";
  cancelText = cancelText ?? "Cancel";


  function editEffect(input) {
    setEffect(Number.isNaN(input) ? 0 : parseInt(input));
  }

  function filterCost() {
    const filteredCost = { ...cost };
    for (const resource in filteredCost) {
      if (filteredCost[resource] < 1) {
        delete filteredCost[resource];
      }
    }
    return filteredCost;
  }

  async function confirmEdit() {
    setSubmitting(true);
    await onConfirm(effect, filterCost());
    setSubmitting(false);
  }

  return (
      <div className="admin-level">
        <div>
          <div>Level:</div>
          <div>{level.level}</div>
        </div>
        <div>
          <div>Effect:</div>
            <input
              type="number"
              value={effect}
              onChange={(e) => editEffect(e.target.value)}
            ></input>
        </div>
        <div>
          <div>Cost:</div>
            <EditCost cost={cost} setCost={setCost} />
        </div>
        <div className="admin-level-actions">
            <>
              <button
                className="button"
                onClick={confirmEdit}
                disabled={submitting}
              >
                {confirmText}
              </button>
              <button
                className="button red"
                onClick={onCancel}
                disabled={submitting}
              >
                {cancelText}
              </button>
            </>
        </div>
      </div>
  );
}

function EditCost({ cost, setCost }) {
  const resources = ["METAL", "SILICONE", "CRYSTAL", "PLUTONIUM"];
  const unusedResources = resources.filter(
    (resource) => !Object.keys(cost).includes(resource)
  );

  function editCost(resource, input) {
    const filteredInput = Number.isNaN(input)
      ? 0
      : Math.max(0, parseInt(input));
    const newCost = { ...cost };
    newCost[resource] = filteredInput;
    setCost(newCost);
  }

  function removeResorce(resource) {
    const newCost = { ...cost };
    delete newCost[resource];
    setCost(newCost);
  }

  function addResource(resource) {
    const newCost = { ...cost };
    newCost[resource] = 0;
    setCost(newCost);
  }

  return (
    <div>
      {Object.keys(cost).map((resource) => (
        <div className="admin-level-cost" key={resource}>
          <div>{resource}:</div>
          <input
            type="number"
            value={cost[resource]}
            onChange={(e) => editCost(resource, e.target.value)}
          />
          <button onClick={() => removeResorce(resource)}>X</button>
        </div>
      ))}
      {unusedResources.length > 0 && (
        <select value={"Add new"} onChange={(e) => addResource(e.target.value)}>
          <option value={"Add new"} disabled>
            Add new
          </option>
          {unusedResources.map((resource) => (
            <option key={resource}>{resource}</option>
          ))}
        </select>
      )}
    </div>
  );
}
