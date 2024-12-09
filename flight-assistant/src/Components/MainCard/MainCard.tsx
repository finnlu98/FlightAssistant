import React from 'react';
import WorldMap from '../WorldMap/WorldMap';
import LiveClock from '../LiveClock/LiveClock';
import { IoIosNotifications } from "react-icons/io";
import { GiIsland } from "react-icons/gi";
import './MainCard.css';

import NextTravelDestination from '../NextTravelDestination/NextTravelDestination';

interface MainCardProps {
    countryIds : string[]
}

const MainCard: React.FC<MainCardProps> = ({ countryIds }) => {

    return (
        <div>
            <div className="container">
                <div className="horizontal-bar-top">
                    <div className='horizontal-bar-top-elements'>
                        <LiveClock />
                        <IoIosNotifications />
                    </div>     
                </div>
                <WorldMap></WorldMap>
                <div className="horizontal-bar-bottom">
                    <div className="horizontal-bar-bottom-elements">
                        <NextTravelDestination></NextTravelDestination>
                        <h5> {countryIds.length} <GiIsland /></h5>
                    </div>     
                </div>
            </div>
        </div>
    );
};


export default MainCard;
