import React, { useState, useEffect, useRef } from 'react';
import { Country, countries } from '../Countries';
import './TravelDestinations.css';
import { FaRegPlusSquare } from "react-icons/fa";
import moment from 'moment';
import TravelDestinationService from '../../../Api/TravelDestinations';
import { RiDeleteBin5Line } from "react-icons/ri";
import HomeButton from '../../Navigation/HomeButton';
import Flag from '../VisitCountries/Flag';


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
                travelDate: moment(destination.travelDate).format("MM/DD/YYYY")
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
            countryCode3: selectedCountry?.code3?.trim() || '',
            countryName: selectedCountry?.name?.trim() || '',
            travelDate: selectedDate ? moment(new Date(selectedDate)).format("YYYY-MM-DD") : ''
        };

        if (!trip.countryCode3 || !trip.countryName || !trip.travelDate) {
            alert("Please select a valid country and travel date before adding the trip.");
            return;
        }
        
        await TravelDestinationService.addTravelDestination(trip.countryCode3, trip.travelDate);

        setTrips((prevTrips) => [...prevTrips, trip]);

        setSearchTerm("");
        setSelectedDate("");

    }

    const handleDeleteClick = async (code3 :string, travelDate: string ) => {

        await TravelDestinationService.deleteTravelDestination(code3, travelDate ? moment(new Date(travelDate)).format("YYYY-MM-DD") : '');

        setTrips((prevTrips) => 
            prevTrips.filter(
                (trip) => !(trip.countryCode3 === code3 && trip.travelDate === travelDate)
            )
        );
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
            <div><HomeButton /></div>
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
                            className='add-icon' 
                            size={50} 
                            color="white"
                            onClick={
                                () => handleAddClick()
                            }/>
                    </div>
                </div>
                <ul className="travel-list">
                    {filteredCountries.map((country) => (
                        <li key={country.name} className="travel-list-item">
                            <label onClick={() => handleCheckboxChange(country)}>
                                {country.name} <div className='flag'><Flag code2={country.code2} /></div> 
                            </label>
                        </li>
                    ))}
                </ul>
            </div>
            <div>
                <h2>Upcoming trips</h2>
                <div className='trips-scrollable-container'>
                    <ul className="travel-list">
                    {trips
                        .slice() 
                        .sort((a, b) => moment(a.travelDate).diff(moment(b.travelDate))) 
                        .map((trip, index) => (
                            <li key={index} className="travel-list-item">
                                <label>
                                    <div>
                                        {trip.countryName} - {moment(trip.travelDate).format("MM/DD/YYYY")}
                                    </div>
                                    <div className='delete-icon'>
                                        <RiDeleteBin5Line onClick={() => handleDeleteClick(trip.countryCode3, trip.travelDate)} />
                                    </div>
                                </label>
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
        </div>
        
    );
};

export default TravelDestinations