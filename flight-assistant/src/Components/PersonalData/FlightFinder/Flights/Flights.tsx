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

    function formatPrice(price: number) {
        return `${price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ')} NOK` ;
    }

    function formatDuration(totalMinutes : number) {
        const hours = Math.floor(totalMinutes / 60);
        const minutes = totalMinutes % 60;
        return `${hours}h ${minutes}m`;
    }

    function formatType(numberLayovers : number, layoverDuration : number) {
        if(numberLayovers == 0) {
            return <div><IoIosRocket size={18}/> Direct</div>
        }

        return <div><TbBuildingAirport size={18}/> {numberLayovers} stop  ({formatDuration(layoverDuration)})</div> ;
    }

    function calculateWidth(subDuration: number, totalDuration: number) {
        return `${((subDuration / totalDuration) * 100).toFixed(2)}%`
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
                
                <ul className='flight-list'>
                    <li className='flight-list-item header'>
                        <div>Trip</div>
                        <div>Details</div>
                        <div>Departure Time</div>
                        <div></div>
                        <div>Arrival Time</div>
                        <div>Price</div>
                    </li>
                    {flights.map((flight) => 
                        <li className='flight-list-item' key={flight.id}>
                            <div>{flight.arrivalAirport}</div>
                            <div className='summary'>
                                <div className='summary-main-text'>{formatDuration(flight.totalDuration) }</div> 
                                <div className='summary-sub-text'>{formatType(flight.numberLayovers, flight.layoverDuration)}</div> 
                            </div>
                            <div>{moment(flight.departureTime).format('D. MMM HH:mm')}</div>
                            <div className='flight-line'>
                                <div className='line-container'>
                                    <span className="line-start">{flight.departureAirport}</span>
                                    <div className="line">
                                    {flight.layovers?.length > 0 &&
                                        flight.layovers.map(layover => (
                                            <div 
                                                key={layover.id} 
                                                className="line-part grey" 
                                                style={{ width: calculateWidth(layover.duration, flight.totalDuration) }}
                                            >
                                                <span className="line-time-grey">{formatDuration(layover.duration)}</span>
                                            </div>
                                        ))}
                                    </div>
                                    <span className="line-end">{flight.arrivalAirport}</span>
                                </div>
                            </div>

                            <div>{moment(flight.arrivalTime).format('D. MMM HH:mm')}</div>
                            <div className='price-container'>
                                <a href={flight.searchUrl} target="_blank" rel="noopener noreferrer">
                                    <button className={`go-button ${
                                        flight.priceRange === PriceRange.Low || flight.hasTargetPrice
                                            ? 'low-price' 
                                            : flight.priceRange === PriceRange.High 
                                            ? 'high-price' 
                                            : ''
                                    }`}>
                                        {formatPrice(flight.price)}
                                    </button>
                                </a>
                            </div> 
                        </li>
                    )}
                </ul>
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