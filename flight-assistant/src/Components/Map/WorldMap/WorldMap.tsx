import React, { useState, useEffect } from 'react';
import { ComposableMap, Geographies, Geography } from "react-simple-maps"
import World from './World.json';
import './WorldMap.css';
import CountryService from '../../../Api/Countries';
import MapService from '../../../Api/MapService';

interface WorldMapProps {
    setNumCountriesVisited: React.Dispatch<React.SetStateAction<number>>;
    mapService: React.MutableRefObject<MapService>;
}

const NOT_VISITED_COLOR: string = "#f792c6"
const VISITED_COLOR: string = "#c8cefa"
const STROKE_COLOR: string = "#fac8e2"

const WorldMap: React.FC<WorldMapProps> = ({setNumCountriesVisited, mapService}) => {

    const [countries, setCountries] = useState<string[]>([]);

    useEffect(() => {
        fetchVisitedCountries();
        mapService.current.onCountryUpdated(updateMap);
      }, []);

    const fetchVisitedCountries = async () => {
        try {
          const countries = await CountryService.getCountries();
          const visitedCountries = countries
                                      .filter(country => country.visited === true)
                                      .map(country => country.code3);

          setCountries(visitedCountries);
          setNumCountriesVisited(visitedCountries.length)
        } catch (err) {
          console.error('Error fetching visited countries:', err);
        }
     };

    const updateMap = (code3: string, visited: boolean) => {
        setCountries((prevCountries) => {
            let updatedCountries;
    
            if (visited) {
                if (!prevCountries.includes(code3)) {
                    updatedCountries = [...prevCountries, code3];
                } else {
                    updatedCountries = prevCountries;
                }
            } else {
                updatedCountries = prevCountries.filter((country) => country !== code3);
            }
    
            setNumCountriesVisited(updatedCountries.length);
    
            return updatedCountries;
        });
    };

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
                                    fill={isVisited ? VISITED_COLOR : NOT_VISITED_COLOR} 
                                    stroke={STROKE_COLOR} 
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