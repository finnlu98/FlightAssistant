import React, { useState, useEffect } from 'react';



const LiveClock = () => {
    const [time, setTime] = useState(new Date());

    useEffect(() => {
        const timerId = setInterval(() => {
            setTime(new Date());
        }, 1000); 

        return () => clearInterval(timerId); 
    }, []);

    const formatMilitaryTime = (date: Date) => {
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');
        return `${hours}:${minutes}`; 
    };

    return (
        <h5>
            {formatMilitaryTime(time)}
        </h5>
    );
};

export default LiveClock