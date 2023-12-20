import Drill from "./ShipParts/Drill";
import Engine from "./ShipParts/Engine";
import Shield from "./ShipParts/Shield";
import ShipStorage from "./ShipParts/ShipStorage/ShipStorage";

export default function DisplayMinerShip({ship, setShip, onClick}) {
    return (<>
            <Shield shield={ship.shield} onClick={onClick}/>
            <Drill drill={ship.drill} onClick={onClick}/>
            <Engine engine={ship.engine} onClick={onClick}/>
            <ShipStorage ship={ship} setShip={setShip} onClick={onClick}/>
    </>);
}
