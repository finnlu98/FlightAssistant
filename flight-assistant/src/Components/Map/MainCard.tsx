import React, { useState, useEffect } from 'react';
import WorldMap from './WorldMap/WorldMap';
import LiveClock from './LiveClock/LiveClock';
import { IoIosNotifications } from "react-icons/io";
import { GiIsland } from "react-icons/gi";
import './MainCard.css';
import NextTravelDestination from './NextTravelDestination/NextTravelDestination';
import moment, { Moment } from 'moment';


const MainCard: React.FC = () => {

    const [countriesVisited, setCountriesVisited] = useState<number>(0)
    const [foundTargetPrice, setFoundTargetPrice] = useState<boolean>(false)
    const [nextDestination, setNextDestionation] = useState<string>("");
    const [nextDateTravel, setNextDateTravel] = useState<Moment>(moment());
    
    function setPlannedTrip(nextDestination : string, nextDate : Date) {
        setNextDestionation(nextDestination);
        setNextDateTravel(moment(nextDate, "YYYY/MM/DD"));
    }

    // Workaround to update text every day
      useEffect(() => {
        const shouldReload = () => {
          const now = moment();
          return (
            now.hour() === 2 &&
            now.minute() === 0 &&
            now.second() === 0
          );
        };
    
        const reloadAtTargetHour = () => {
          if (shouldReload()) {
            window.location.reload();
          }
        };
    
        reloadAtTargetHour();
    
        const intervalId = setInterval(reloadAtTargetHour, 1000);
    
        return () => clearInterval(intervalId);
      }, []);

    return (
        <div>
            <div className="container">
                <div className="horizontal-bar-top">
                    <div className='horizontal-bar-top-elements'>
                        <LiveClock />
                        <div className={`notification-icon-container ${foundTargetPrice ? 'active' : ''}`}>
                            <IoIosNotifications className="notification-icon" />
                        </div>
                    </div>     
                </div>
                <WorldMap 
                    setCountriesVisited={setCountriesVisited} 
                    setFoundTargetPrice={setFoundTargetPrice}
                    setPlannedTrip={setPlannedTrip}/>
                <div className="horizontal-bar-bottom">
                    <div className="horizontal-bar-bottom-elements">
                        <NextTravelDestination 
                            nextDestination={nextDestination} 
                            nextDateTravel={nextDateTravel}
                            setPlannedTrip={setPlannedTrip}/>
                        
                        <h5> {countriesVisited} <GiIsland /></h5>
                    </div>     
                </div>
            </div>
        </div>
    );
};


export default MainCard;
