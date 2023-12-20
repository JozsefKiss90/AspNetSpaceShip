import { useNavigate } from "react-router-dom";
import { useNotificationsDispatch } from "../components/notifications/NotificationContext";
import { useCallback } from "react";

export default function useHandleFetchError() {
    const navigate = useNavigate();
    const notifDispatch = useNotificationsDispatch();

    return useCallback(async (response) => {
        if (response.status === 403) {
            navigate("/403");
        } else if (response.status === 404) {
            navigate("/404");
        } else {
            let message;
            try {
                const data = await response.json();
                if (data.detail) {
                    message = data.detail;
                } else throw new Error();
            } catch (err) {
                message = response.statusText;
            }
            notifDispatch({
                type: "add",
                message,
                notifType: "error",
                timer: 5
            });
        }
    }, [notifDispatch]);
}