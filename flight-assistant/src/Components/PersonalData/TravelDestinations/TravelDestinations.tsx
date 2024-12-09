import React, { useState, useEffect, useRef } from 'react';
import { countries } from '../Countries';
import './TravelDestinations.css';
import { FaRegPlusSquare } from "react-icons/fa";
import moment from 'moment';


interface TravelDestinationsProps {
    countryIds : string[]
}

interface Trip {
    countryCode3: string
    countryName: string
    travelDate: Date
}

const TravelDestinations: React.FC<TravelDestinationsProps> = ({ countryIds }) => {
    const [selectedCountry, setSelectedCountry] = useState<string>("");
    const [selectedDate, setSelectedDate] = useState<string>("");
    const [searchTerm, setSearchTerm] = useState<string>('');
    const [trips, setTrips] = useState<Trip[]>([]);

    const handleCheckboxChange = (country: string) => {
        setSelectedCountry(country);
        setSearchTerm(country);
    };

    const handleAddClick = (country: string) => {
        const trip : Trip = {
            countryCode3 : "test",
            countryName: country,
            travelDate: new Date(selectedDate)
        }
        
        setTrips((prevTrips) => [...prevTrips, trip]);
    }

    const filteredCountries = countries.filter((country) => {
        const lowerCaseSearchTerm = searchTerm.toLowerCase();
        const lowerCaseCountryName = country.name.toLowerCase();
    
        if (lowerCaseSearchTerm === lowerCaseCountryName) {
            return false;
        }

        if (lowerCaseSearchTerm === "") {
            return false;
        }
    
        return lowerCaseCountryName.includes(lowerCaseSearchTerm);
    });

    return (
        <div className='country-select-container'>
            <div className="country-select">
                <div className='country-select-header'>
                    <h2>Select Country to Visit</h2>

                    <div className='input-container'>
                        <input
                        type="text"
                        placeholder="Set Destination..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="search-input"
                        />
                        <input
                            type="date"
                            value={selectedDate}
                            onChange={(e) => setSelectedDate(e.target.value)}
                            className="date-picker"
                        />
                        <FaRegPlusSquare 
                            size={50} 
                            color="white"
                            onClick={
                                () => handleAddClick(searchTerm)
                            }/>
                    </div>
                    

                </div>

                <ul className="country-list-select">
                    {filteredCountries.map((country) => (
                        <li key={country.name} className="country-list-item">
                            <label onClick={() => handleCheckboxChange(country.name)}>
                                
                                {country.name}
                            </label>
                        </li>
                    ))}
                </ul>
            </div>
            <div>
                <h2>Upcoming trips</h2>
                <ul className="country-list-select">
                    {trips.map((trip) => (
                        <li key={trip.countryName} className="country-list-item">
                            <label>
                                {trip.countryName} - {moment(trip.travelDate).format("DD.MM.YYYY")}
                            </label>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
        
    );
};

export default TravelDestinations