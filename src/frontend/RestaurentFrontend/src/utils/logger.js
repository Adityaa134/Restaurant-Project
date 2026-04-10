const isDev = import.meta.env.DEV;
console.log(isDev)
export const logger = {
    log: (...args) =>{
        if(isDev)
        {
            console.log(...args);
        }
    },
    error: (message, details = {}) =>{
        if(isDev)
        {
            console.error(`ERROR: ${message}`,details);
        }
    },
    warn: (message, details = {}) =>{
        if(isDev)
        {
            console.error(`WARN: ${message}`,details);
        }
    }
};