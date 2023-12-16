import Engine from "./ShipParts/Engine";
import Scanner from "./ShipParts/Scanner";
import Shield from "./ShipParts/Shield";

export default function DisplayScoutShip({ship, onClick}) {
    return (<>
            <Shield shield={ship.shield} onClick={onClick}/>
            <Engine engine={ship.engine} onClick={onClick}/>
            <Scanner scanner={ship.scanner} onClick={onClick}/>
    </>);
}