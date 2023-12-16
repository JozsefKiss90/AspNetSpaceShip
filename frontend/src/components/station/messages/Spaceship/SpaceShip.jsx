import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import useHandleFetchError from "../../../../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "../../../notifications/NotificationContext";
import { useStorageDispatchContext } from "../../StorageContext";
import DisplayMinerShip from "./DisplayMinerShip";
import DisplaySpaceShip from "./DisplaySpaceShip";
import DisplayScoutShip from "./DisplayScoutShip";
import { getAuthHeaders } from "../../../notifications/NotificationContext";

export default function SpaceShip() {
  const { id } = useParams();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const storageSetter = useStorageDispatchContext();
  const navigate = useNavigate();
  const [ship, setShip] = useState(null);
  const [part, setPart] = useState(null);
  const [cost, setCost] = useState(null);
  const fetchShip = useCallback(async () => {
    setShip(null);
    try {
      const res = await fetch(`/api/v1/ship/${id}`, {
        headers: {
            ...getAuthHeaders(),
            'Content-Type': 'application/json'
        }
    });
      if (res.ok) {
        const data = await res.json();
        setShip(data);
        setPart(null);
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
  }, [id, notifDispatch, handleFetchError]);

  useEffect(() => {
    fetchShip();
  }, [fetchShip]);

  useEffect(() => {
    if (ship !== null && !["SCOUT", "MINER"].includes(ship.type)) {
      notifDispatch({
        type: "add",
        message: "Unrecognized ship type.",
        notifType: "error",
        timer: 10,
      });
      navigate("..");
    }
  }, [ship, notifDispatch, navigate]);

  async function getShipPartUpgradeCost(part) {
    setCost(null);
    try {
      const res = await fetch(`/api/v1/ship/${ship.id}/upgrade?part=${part}`);
      if (res.ok) {
        const data = await res.json();
        setCost(data);
        return true;
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
    return false;
  }

  async function onClick(part) {
    setPart(null);
    if (await getShipPartUpgradeCost(part)) {
      setPart(part);
    }
  }

  async function upgradePart() {
    try {
      const res = await fetch(`/api/v1/ship/${ship.id}/upgrade?part=${part}`, {
        method: "PATCH",
      });
      if (res.ok) {
        const data = await res.json();
        setShip(data);
        setPart(null);
        setCost(null);
        storageSetter({ type: "update" });
        notifDispatch({
          type: "add",
          message: `${part} upgraded.`,
          timer: 5,
        });
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
  }

  const spaceShipProps = {
    ship,
    setShip,
    cost,
    part,
    upgradePart,
  };

  if (ship === null) {
    return <div>Loading...</div>;
  } else if (ship.type === "MINER") {
    return (
      <DisplaySpaceShip {...spaceShipProps}>
        <DisplayMinerShip ship={ship} setShip={setShip} onClick={onClick} />
      </DisplaySpaceShip>
    );
  } else if (ship.type === "SCOUT") {
    return (
      <DisplaySpaceShip {...spaceShipProps}>
        <DisplayScoutShip ship={ship} onClick={onClick} />
      </DisplaySpaceShip>
    );
  }
}
