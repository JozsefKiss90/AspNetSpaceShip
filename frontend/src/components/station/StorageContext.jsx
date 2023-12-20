import {createContext, useContext, useEffect, useReducer, useState} from 'react';

const StorageContext = createContext(null);
const StorageDispatchContext = createContext(null);

export function useStorageContext() {
    return useContext(StorageContext);
}
export function useStorageDispatchContext() {
    return useContext(StorageDispatchContext);
}

export function StorageStationContext({children}) {
    const [storage, dispatch] = useReducer(StorageReducer, false);

    return (<>
        <StorageContext.Provider value={storage}>
            <StorageDispatchContext.Provider value={dispatch}>
                {children}
            </StorageDispatchContext.Provider>
        </StorageContext.Provider>
    </>);
}


async function StorageReducer(storage, action) {
    switch (action.type) {
        case 'update': {
           return !storage;
        }
    }
}


