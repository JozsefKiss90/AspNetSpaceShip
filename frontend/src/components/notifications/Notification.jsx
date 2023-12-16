import { useCallback, useEffect } from "react";
import { useNotificationsDispatch } from "./NotificationContext";

export default function Notification({ notification }) {
    const notifDispatch = useNotificationsDispatch();

    const close = useCallback(() => {
        notifDispatch({
            type: "remove",
            id: notification.id
        });
    }, [notification, notifDispatch]);

    useEffect(() => {
        let timer;
        if (notification.timer < 1) {
            return;
        } else if (notification.timer === undefined) {
            timer = 10;
        } else {
            timer = notification.timer;
        }
        setTimeout(() => close(), timer * 1000);
    }, [notification, close])

    let background;
    switch (notification.type) {
        case "error":
            background = {
                background: "linear-gradient(#420000e6, #be0000ea)",
                boxShadow: "0 0 2px 2px rgba(77, 0, 0, 0.741)"
            };
            break;
        case "admin":
            background = {
                background: "linear-gradient(#00065ae6, #003fbeea)",
                boxShadow: "0 0 2px 2px rgba(4, 0, 77, 0.741)"
            };
            break;
        default:
            background = {
                background: "linear-gradient(#004b00ec, #006400ea)",
                boxShadow: "0 0 2px 2px rgba(29, 52, 0, 0.741)"
            };
    }
    return <div className="notification" style={background}>
        {notification.message}
        <div>
            <button onClick={close}>X</button>
        </div>
    </div>
}