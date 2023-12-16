import Notification from "./Notification";
import { useNotifications } from "./NotificationContext";
import "./Notifications.css";

export default function Notifications() {
    const notifications = useNotifications();

    return <div className="notifications">
        {notifications.map(n => <Notification key={n.id} notification={n} />)}
    </div>
}