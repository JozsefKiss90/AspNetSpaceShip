import "./ShipName.css";
import { useHangarDispatchContext } from "../../../HangarContext";
import { useEffect, useState } from "react";
import useHandleFetchError from "../../../../../hooks/useHandleFetchError";
import { useNotificationsDispatch, getAuthHeaders } from "../../../../notifications/NotificationContext";

export function ShipName({ ship, setShip }) {
    const hangarSetter = useHangarDispatchContext();
    const handleFetchError = useHandleFetchError();
    const notifDispatch = useNotificationsDispatch();
    const [nameInputDisplay, setNameInputDisplay] = useState(false);
    const [shipName, setShipName] = useState(ship.name);
    const [submitting, setSubmitting] = useState(false);

    async function renameShip(name) {
        if (name === ship.name) {
            setNameInputDisplay(false);
            return;
        }
        setSubmitting(true);
        try {

            const res = await fetch(`/api/v1/ship/${ship.id}`, {
                method: "PATCH",
                headers: {
                    ...getAuthHeaders(),
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    name: name,
                }),
            });
            if (res.ok) {
                setShip({
                    ...ship,
                    name
                });
                setNameInputDisplay(false);
                hangarSetter({ type: "update" });
                notifDispatch({
                    type: "add",
                    message: "Ship renamed.",
                    timer: 5
                });
            } else {
                handleFetchError(res);
            }
        } catch (err) {
            console.error(err);
            notifDispatch({
                type: "generic error"
            })
        }
        setSubmitting(false);
    }

    useEffect(() => {
        setShipName(ship.name);
    }, [ship])


    return (<>
        <div className="ship-name row">
            {nameInputDisplay
                ? <div className="name-input-div">
                    <input className="name-input"
                        type="text"
                        value={shipName}
                        onChange={(e) => setShipName(e.target.value)}
                        placeholder="Enter ship name"
                        size="9"
                        minLength="3"
                        maxLength="15"
                    ></input>
                    <button className="button" onClick={() => renameShip(shipName)} disabled={submitting}>
                        Change!
                    </button>
                </div>
                : <>
                    <div>{ship.name}</div>
                    <button className="button" onClick={() => {
                        setNameInputDisplay(true);
                    }}>Rename
                    </button>
                </>
            }
        </div>
    </>)

}
