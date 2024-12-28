import React, { useEffect, useState } from 'react';
import FlightService from '../../../../Api/Flights';
import { Flight, PriceRange } from '../../../../Models/Flight';
import './Flights.css';
import moment from 'moment';
import { FaTelegramPlane } from "react-icons/fa";
import FlightQueries from '../FlightQueries/FlightQueries';
import { FaTimes } from 'react-icons/fa';
import HomeButton from '../../../Navigation/HomeButton';
import { TbBuildingAirport } from "react-icons/tb";
import { IoIosRocket } from "react-icons/io";




interface Flights {
    
}

const Flights: React.FC = () => {
    const [flights, setFlights] = useState<Flight[]>([]);
    const [isPopupOpen, setIsPopUpOpen] = useState<boolean>(false);

    useEffect(() => {
        const fetchFlights = async () => {
            try {
                const storedFlights = await FlightService.getFlights();
                //setFlights(storedFlights);

                sortFlights(storedFlights);

            } catch (err) {
                console.error('Error fetching flights:', err);
            }
        };

        const notifyReadFlights = async () => {
            try {
               await FlightService.notifyReadFlights();
               
            } catch (err) {
                console.error('Error fetching flights:', err);
            }
        };

        fetchFlights();
        notifyReadFlights();
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
            return <div><IoIosRocket size={18}/> (Direct)</div>
        }

        return <div>{numberLayovers} <TbBuildingAirport size={20}/> ({formatDuration(layoverDuration)})</div> ;
    }

    function sortFlights(storedFlights: Flight[]) { 
        var sortedFlights = storedFlights.slice().sort((a, b) => {
            const createdAtComparison = moment(b.createdAt).diff(moment(a.createdAt));
            
            if (createdAtComparison !== 0) {
                return createdAtComparison;
            }
            
            return moment(a.departureTime).diff(moment(b.departureTime));
        });

        setFlights(sortedFlights)
    }

    return (
        <div className="flight-container">
            <div className='headers'>
                <div className='headers-elements'>
                    <div><HomeButton /></div>
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
                                <tr key={flight.id} className={`${flight.priceRange === PriceRange.Low || flight.hasTargetPrice ? 'low-price' : flight.priceRange === PriceRange.High ? 'high-price' : ''}`}>
                                    <td>{flight.departureAirport}</td>
                                    <td>{flight.arrivalAirport}</td>
                                    <td>{moment(flight.departureTime).format('MM/DD/YYYY HH:mm')}</td>
                                    <td>{moment(flight.arrivalTime).format('MM/DD/YYYY HH:mm')}</td>
                                    <td>{formatDuration(flight.totalDuration)}</td>
                                    <td>{formatType(flight.numberLayovers, flight.layoverDuration) }</td>
                                    <td className={`priceCell`}>
                                        <span className="priceText">
                                            {flight.price}
                                        </span>
                                        <a href={flight.searchUrl} target='_blank' rel='noopener noreferrer'>
                                            <FaTelegramPlane />
                                        </a>
                                    </td>
                                    
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={7}>No flights available</td>
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