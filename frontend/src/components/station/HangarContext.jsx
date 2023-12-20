import {createContext, useContext, useReducer} from 'react';

const HangarContext = createContext(null);
const HangarDispatchContext = createContext(null);

export function useHangarContext() {
    return useContext(HangarContext);
}
export function useHangarDispatchContext() {
    return useContext(HangarDispatchContext);
}

export function HangarStationContext({children}) {
    const [hangar, dispatch] = useReducer(HangarReducer, false);

    return (<>
        <HangarContext.Provider value={hangar}>
            <HangarDispatchContext.Provider value={dispatch}>
                {children}
            </HangarDispatchContext.Provider>
        </HangarContext.Provider>
    </>);
} 


async function HangarReducer(hangar, action) {
    switch (action.type) {
        case 'update': {
           return !hangar;
        }
    }
}


