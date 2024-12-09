import React, { useState, useEffect } from 'react';
import { ComposableMap, Geographies, Geography } from "react-simple-maps"
import World from './World.json';
import './WorldMap.css';
import CountryService from '../../Api/Countries';

const WorldMap: React.FC = () => {

    const [countries, setCountries] = useState<string[]>([]);

    var notVisited = "#f792c6"
    var visited = "#c8cefa"

    useEffect(() => {
        const fetchVisitedCountries = async () => {
          try {
            const visitedCountries = await CountryService.getCountries();
            const filteredCountries = visitedCountries
                                        .filter(country => country.visited === true)
                                        .map(country => country.code3);
            setCountries(filteredCountries);
          } catch (err) {
            console.error('Error fetching visited countries:', err);
          }
        };
    
        fetchVisitedCountries();
      }, []);


    return (
        <div className='container'>
            <ComposableMap className="map">
                <Geographies geography={World}>
                    {({ geographies }) =>
                        geographies.map((geo) => {
                            const isVisited = countries.includes(geo.id); 
                            
                            return (
                                <Geography 
                                    key={geo.rsmKey} 
                                    geography={geo} 
                                    fill={isVisited ? visited : notVisited} 
                                    stroke="#fac8e2" 
                                />
                            );
                        })
                    }
                </Geographies>
            </ComposableMap>
        </div>
    );
};




export default WorldMap;