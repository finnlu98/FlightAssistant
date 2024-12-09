import React, { useState, useEffect } from 'react';



const LiveClock = () => {
    const [time, setTime] = useState(new Date());

    useEffect(() => {
        const timerId = setInterval(() => {
            setTime(new Date());
        }, 1000); // Updates every second

        return () => clearInterval(timerId); // Cleanup on component unmount
    }, []);

    return (
        <h5>
            {time.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })} 
        </h5>
    );
};

export default LiveClock