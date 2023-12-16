import { createContext, useContext, useReducer } from "react";
import { v4 as uuidv4 } from 'uuid';

const NotificationContext = createContext(null);
const NotificationDispatchContext = createContext(null); 

export function NotificationProvider({ children }) {
    const [notfications, dispatch] = useReducer(notificationReducer, []);

    return (
        <NotificationContext.Provider value={notfications}>
            <NotificationDispatchContext.Provider value={dispatch}>
                {children}
            </NotificationDispatchContext.Provider>
        </NotificationContext.Provider>
    );
}

export function getAuthHeaders() {
  const jwtToken = localStorage.getItem('jwt');
  return jwtToken ? { 'Authorization': `Bearer ${jwtToken}` } : {};
}

export function useNotifications() {
    return useContext(NotificationContext);
}

export function useNotificationsDispatch() {
    return useContext(NotificationDispatchContext);
}

function notificationReducer(notifications, action) {
    switch (action.type) {
        case 'add': {
            return [...notifications, {
                id: uuidv4(),
                type: action.notifType,
                message: action.message,
                timer: action.timer
            }];
        }
        case 'remove': {
            return notifications.filter(n => n.id !== action.id);
        }
        case 'generic error': {
            return [...notifications, {
                id: uuidv4(),
                type: 'error',
                message: 'Something went wrong.',
                timer: 10
            }]
        }
        default: {
            throw new Error(`Unknown action: ${action.type}`);
        }
    }
}