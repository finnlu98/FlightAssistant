import React, { useState, useEffect, useRef } from 'react';
import { Country, countries } from '../Countries';
import './TravelDestinations.css';
import { FaRegPlusSquare } from "react-icons/fa";
import moment from 'moment';
import TravelDestinationService from '../../../Api/TravelDestinations';


interface Trip {
    countryCode3: string
    countryName: string
    travelDate: string
}

const TravelDestinations: React.FC = () => {
    const [selectedCountry, setSelectedCountry] = useState<Country | null >(null);
    const [selectedDate, setSelectedDate] = useState<string>("");
    const [searchTerm, setSearchTerm] = useState<string>('');
    const [trips, setTrips] = useState<Trip[]>([]);


    useEffect(() => {
        const fetchTravelDestinations = async () => {
          try {
            const travelDestinations = await TravelDestinationService.getTravelDestinations();
            const formattedDestinations = travelDestinations.map(destination => ({
                countryCode3: destination.code3,
                countryName: destination.country.name,
                travelDate: moment(destination.travelDate).format("YYYY-MM-DD")
            }));

            setTrips(formattedDestinations);

          } catch (err) {
            console.error('Error fetching visited countries:', err);
          }
        };
    
        fetchTravelDestinations();
      }, []);



    const handleCheckboxChange = (country: Country) => {
        setSelectedCountry(country);
        setSearchTerm(country.name);
    };

    const handleAddClick = async () => {
        const trip: Trip = {
            countryCode3: selectedCountry ? selectedCountry.code3 : '',
            countryName: selectedCountry ? selectedCountry.name : '',
            travelDate: moment(new Date(selectedDate)).format("YYYY-MM-DD") 
        };
        
        await TravelDestinationService.addTravelDestination(trip.countryCode3, trip.travelDate);

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
                                () => handleAddClick()
                            }/>
                    </div>
                    

                </div>

                <ul className="country-list-select">
                    {filteredCountries.map((country) => (
                        <li key={country.name} className="country-list-item">
                            <label onClick={() => handleCheckboxChange(country)}>
                                
                                {country.name}
                            </label>
                        </li>
                    ))}
                </ul>
            </div>
            <div>
                <h2>Upcoming trips</h2>
                <ul className="country-list-select">
                    {trips.map((trip, index) => (
                        <li key={index} className="country-list-item">
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