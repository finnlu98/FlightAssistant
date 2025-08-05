import React, { useState, useEffect, useRef } from 'react';
import WorldMap from './WorldMap/WorldMap';
import LiveClock from './LiveClock/LiveClock';
import { IoIosNotifications } from "react-icons/io";
import { GiIsland } from "react-icons/gi";
import './MainCard.css';
import NextTravelDestination from './NextTravelDestination/NextTravelDestination';
import moment, { Moment } from 'moment';
import MapService from '../../Api/MapService';

const MainCard: React.FC = () => {

    const mapService = useRef<MapService>(new MapService());

    const [numCountriesVisited, setNumCountriesVisited] = useState<number>(0)
    const [foundTargetPrice, setFoundTargetPrice] = useState<boolean>(false)

    useEffect(() => {
      const timeoutId = scheduleReload();
      return () => clearInterval(timeoutId);
     }, []);

    const scheduleReload = () => {
      const now = moment();
      let nextReload = moment().hour(2).minute(0).second(0);
    
      if (now.isAfter(nextReload)) {
        nextReload.add(1, "day");
      }
    
      const msUntilReload = nextReload.diff(now);
    
      const timeoutId = setTimeout(() => {
        window.location.reload();
      }, msUntilReload);
    
      return timeoutId;
    };

    useEffect(() => {
        mapService.current.onNotifyTargetPrice(setFoundTargetPrice);
        return () => {
          mapService.current.stopConnection();
        };
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
                <WorldMap setNumCountriesVisited={setNumCountriesVisited} mapService={mapService}/>
                <div className="horizontal-bar-bottom">
                    <div className="horizontal-bar-bottom-elements">
                        <NextTravelDestination mapService={mapService}/>
                        <h5> {numCountriesVisited} <GiIsland /></h5>
                    </div>     
                </div>
            </div>
        </div>
    );
};

export default MainCard;
