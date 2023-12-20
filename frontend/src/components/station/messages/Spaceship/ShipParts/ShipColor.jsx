import { useEffect, useState } from "react";
import "./ShipColor.css";
import useHandleFetchError from "../../../../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../../../../notifications/NotificationContext";

export function ShipColor({ ship, setShip }) {
    const notifDispatch = useNotificationsDispatch();
    const handleFetchError = useHandleFetchError();
    const [allShipColors, setAllShipColors] = useState(null);
    const [color, setColor] = useState(ship.color);
    const [colorListDisplay, setColorListDisplay] = useState(false);
    const [submitting, setSubmitting] = useState(false);

    async function changeColor() {
        if (color === ship.color) {
            setColorListDisplay(false);
        } else {
            try {
                setSubmitting(true);
                const res = await fetch(`/api/v1/ship/${ship.id}`, {
                    method: "PATCH",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({
                        color: color.toUpperCase(),
                    }),
                });
                if (res.ok) {
                    const newShip = { ...ship };
                    newShip.color = color;
                    setShip(newShip);
                    setColorListDisplay(false);
                    notifDispatch({
                        type: "add",
                        message: "Ship repainted.",
                        timer: 5
                    });
                } else {
                    handleFetchError(res);
                }
            } catch (err) {
                console.error(err);
                notifDispatch({
                    type: "generic error"
                });
            }
            setSubmitting(false);
        }
    }

    async function getAvailableColors() {
        if (!allShipColors) {
            try {
                const res = await fetch(`/api/v1/ship/color`);
                if (res.ok) {
                    const data = await res.json();
                    setAllShipColors(data);
                    setColorListDisplay(true);
                } else {
                    handleFetchError(res);
                }
            } catch (err) {
                console.error(err);
                notifDispatch({
                    type: "generic error"
                });
            }
        } else {
            setColorListDisplay(true);
        }
    }

    useEffect(() => {
        setColor(ship.color);
    }, [ship])

    function ColorList() {
        return (
            <div className="ship-color row">
                <select name="colors" value={color} onChange={(e) => {
                    setColor(e.target.value);
                }}>
                    {allShipColors.map((colorM) => (
                        <option key={colorM} value={colorM}>
                            {colorM}
                        </option>
                    ))}
                </select>
                <button className="button" onClick={changeColor} disabled={submitting}> Choose</button>
            </div>);
    }

    return (
        <>
            {colorListDisplay
                ? <ColorList />
                : (<div className="ship-color row">
                    <div> Color: {ship.color.toLowerCase()}</div>
                    <button className="button" onClick={getAvailableColors}>Change</button>
                </div>)}
        </>)
}
