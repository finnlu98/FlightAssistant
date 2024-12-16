import React, { useState } from 'react';
import WorldMap from './WorldMap/WorldMap';
import LiveClock from './LiveClock/LiveClock';
import { IoIosNotifications } from "react-icons/io";
import { GiIsland } from "react-icons/gi";
import './MainCard.css';
import NextTravelDestination from './NextTravelDestination/NextTravelDestination';

const MainCard: React.FC = () => {

    const [countriesVisited, setCountriesVisited] = useState<number>(0)
    const [foundTargetPrice, setFoundTargetPrice] = useState<boolean>(false)

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
                <WorldMap setCountriesVisited={setCountriesVisited} setFoundTargetPrice={setFoundTargetPrice}></WorldMap>
                <div className="horizontal-bar-bottom">
                    <div className="horizontal-bar-bottom-elements">
                        <NextTravelDestination></NextTravelDestination>
                        <h5> {countriesVisited} <GiIsland /></h5>
                    </div>     
                </div>
            </div>
        </div>
    );
};


export default MainCard;
