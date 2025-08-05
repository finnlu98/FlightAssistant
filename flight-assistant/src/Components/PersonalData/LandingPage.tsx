import React from 'react';
import './LandingPage.css'
import { FaPlaneDeparture } from 'react-icons/fa';
import { FaMapMarkedAlt, FaClipboardList } from 'react-icons/fa';
import { IoPaperPlane } from "react-icons/io5";


const LandingPage: React.FC = () => {

    return (
        <div className="landing-page-wrapper">
        <h1 className="title">Flight Assistant 3000</h1>
        <FaPlaneDeparture className="plane-icon" />
        <div className="landing-page-container">
            <div>
                <a href='countries'>
                    <FaMapMarkedAlt className="icon" />
                    <h2>Countries Visited</h2>
                </a>
            </div>
            <div>
                <a href='destination'>
                    <FaClipboardList className="icon" />
                    <h2>Set Trips</h2>
                </a>
            </div>
            <div>
                <a href='flights'>
                    <IoPaperPlane className="icon" />
                    <h2>Flights</h2>
                </a>

            </div>
        </div>
    </div>
    );
};


export default LandingPage;