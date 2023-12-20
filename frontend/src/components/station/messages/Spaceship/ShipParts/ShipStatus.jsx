import {useNavigate} from "react-router-dom";

export function ShipStatus({ship}) {
    const navigate = useNavigate();

    return (<div className="ship-status row">
        {ship.missionId > 0
            ? <>
                <div>Status: On mission</div>
                <div className="button" onClick={() => navigate(`/station/mission/${ship.missionId}`)}>Show</div>
            </>
            : <div>Status: In dock</div>}
    </div>)
}
