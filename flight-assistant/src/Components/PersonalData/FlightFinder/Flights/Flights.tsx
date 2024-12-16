import React, { useEffect, useState } from 'react';
import FlightService from '../../../../Api/Flights';
import { Flight } from '../../../../Models/Flight';
import './Flights.css';
import moment from 'moment';
import { FaArrowLeft } from "react-icons/fa";
import { FaTelegramPlane } from "react-icons/fa";
import FlightQueries from '../FlightQueries/FlightQueries';
import { FaTimes } from 'react-icons/fa';


interface Flights {
    
}

const Flights: React.FC = () => {
    const [flights, setFlights] = useState<Flight[]>([]);
    const [isPopupOpen, setIsPopUpOpen] = useState<boolean>(false);

    useEffect(() => {
        const fetchFlights = async () => {
            try {
                const storedFlights = await FlightService.getFlights();
                setFlights(storedFlights);
            } catch (err) {
                console.error('Error fetching flights:', err);
            }
        };

        fetchFlights();
    }, []);

    function setPopUpOpen() {
        setIsPopUpOpen(!isPopupOpen)
    }

    function formatDuration(totalMinutes : number) {
        const hours = Math.floor(totalMinutes / 60);
        const minutes = totalMinutes % 60;
        return `${hours}h ${minutes}m`;
    }

    function formatType(numberLayovers : number, layoverDuration : number) {
        if(numberLayovers == 0) {
            return "Direct"
        }

        return `${numberLayovers} layovers (${formatDuration(layoverDuration)})`;
    }

    return (
        <div className="flight-container">
            <div className='headers'>
                <div className='headers-elements'>
                    <div><FaArrowLeft /></div>
                    <h1>Flight Finder</h1>
                    <div>
                        <button className="edit-button" onClick={() => setPopUpOpen()}>Edit queries</button>  
                    </div>
                </div>
            </div>
            <div className="table-wrapper">
                <table cellPadding="10" cellSpacing="0">
                    <thead>
                        <tr>
                            <th>Departure</th>
                            <th>Arrival</th>
                            <th>Departure Time</th>
                            <th>Arrival Time</th>
                            <th>Duration</th>
                            <th>Type</th>
                            <th>Price</th>


                            
                        </tr>
                    </thead>
                    <tbody>
                        {flights.length > 0 ? (
                            flights.map((flight) => (
                                <tr key={flight.id}>
                                    <td>{flight.departureAirport}</td>
                                    <td>{flight.arrivalAirport}</td>
                                    <td>{moment(flight.departureTime).format('YYYY-MM-DD HH:mm')}</td>
                                    <td>{moment(flight.arrivalTime).format('YYYY-MM-DD HH:mm')}</td>
                                    <td>{formatDuration(flight.totalDuration)}</td>
                                    <td>{formatType(flight.numberLayovers, flight.layoverDuration) }</td>
                                    <td className='priceCell'>{flight.price} <a href={flight.searchUrl} target='_blank' rel='noopener noreferrer'><FaTelegramPlane /></a></td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={5}>No flights available</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
            
            {isPopupOpen &&(
                <div className="popup-overlay">
                    <div className="popup-content">
                    <button className="close-button" onClick={setPopUpOpen}>
                            <FaTimes size={20} />
                        </button>
                        <FlightQueries />
                    </div>
                </div>
            )}
            
        </div>
    );
};

export default Flights;