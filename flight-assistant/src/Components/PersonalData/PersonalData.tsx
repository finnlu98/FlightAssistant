import React, { useState, useEffect, useRef } from 'react';
import { countries } from './Countries';
import './PersonalData.css';
import CountryService from '../../Api/Countries';

interface PersonalDataProps {
    countryIds : string[]
}

const PersonalData: React.FC<PersonalDataProps> = ({ countryIds }) => {
    const [selectedCountries, setSelectedCountries] = useState<string[]>([]);
    const [searchTerm, setSearchTerm] = useState<string>('');

    const handleCheckboxChange = (country: string, visited : boolean) => {
        setSelectedCountries((prevSelected) =>
            prevSelected.includes(country)
                ? prevSelected.filter((c) => c !== country)
                : [...prevSelected, country]
        );

        CountryService.setVisitedCountry(country, visited)
    };

    useEffect(() => {
        const fetchVisitedCountries = async () => {
          try {
            const visitedCountries = await CountryService.getCountries();
            const filteredCountries = visitedCountries
                                        .filter(country => country.visited === true)
                                        .map(country => country.code3);
            
            setSelectedCountries(filteredCountries);
          } catch (err) {
            console.error('Error fetching visited countries:', err);
          }
        };
    
        fetchVisitedCountries();
      }, []);

        const filteredCountries = countries.filter((country) =>
        country.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="country-selector">
            <div className='country-selector-header'>
                <h2>Select Countries Visited</h2>

                <input
                type="text"
                placeholder="Search countries..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="search-input-selector"
                />
            </div>
            

            <ul className="country-list">
                {filteredCountries.map((country) => (
                    <li key={country.name} className="country-list-item">
                        <label>
                            <input
                                type="checkbox"
                                className="country-checkbox"
                                checked={selectedCountries.includes(country.code3)}
                                onChange={(e) => handleCheckboxChange(country.code3, e.target.checked)}
                            />
                            {country.name}
                        </label>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default PersonalData