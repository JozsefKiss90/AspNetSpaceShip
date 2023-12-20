import { ShipColor } from "./ShipParts/ShipColor";
import { ShipName } from "./ShipParts/ShipName";
import { ShipStatus } from "./ShipParts/ShipStatus";
import { ResourceNeeded } from "../ResourceNeeded";
import "./DisplaySpaceShip.css";

export default function DisplaySpaceShip({ children, ship, setShip, cost, part, upgradePart }) {
    
  const type = ship.type.toLowerCase();

  return (
    <>
      <div
        className="container"
        style={{ display: "flex", flexFlow: "column" }}
      >
        <ShipName ship={ship} setShip={setShip} />
        <div className="ship-type" style={{ fontSize: "13px", height: "15px" }}>
          {type} ship
        </div>
        <ShipColor ship={ship} setShip={setShip} />
        <ShipStatus ship={ship} />
        {children}
      </div>
      <div className="side-bar">
        <img
          alt="spaceship"
          src="/ship_five.png"
          style={{
            width: "128px",
            height: "128px",
            padding: "60px",
            justifySelf: "end",
            alignSelf: "end",
          }}
        />
        <div
          className="resource-needed"
          style={{ height: "min-content", paddingTop: "15px" }}
        >
          {part ? (
            <ResourceNeeded
              cost={cost}
              item={part.toLowerCase()}
              onConfirm={upgradePart}
            />
          ) : (
            <></>
          )}
        </div>
      </div>
    </>
  );
}
