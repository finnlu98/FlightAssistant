import React, { useState, useEffect } from 'react';
import { ComposableMap, Geographies, Geography } from "react-simple-maps"
import World from './World.json';
import './WorldMap.css';
import CountryService from '../../../Api/Countries';
import * as signalR from '@microsoft/signalr';

const MAP_CONNECTION_URL = `${process.env.REACT_APP_SERVER_MAP_URL}`;


interface WorldMapProps {
    setCountriesVisited: React.Dispatch<React.SetStateAction<number>>;
    setFoundTargetPrice: React.Dispatch<React.SetStateAction<boolean>>;

}

const WorldMap: React.FC<WorldMapProps> = ({setCountriesVisited, setFoundTargetPrice}) => {

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
            setCountriesVisited(filteredCountries.length)
          } catch (err) {
            console.error('Error fetching visited countries:', err);
          }
        };
    
        fetchVisitedCountries();
      }, []);


      useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
          .withUrl(MAP_CONNECTION_URL, {
            withCredentials: true,
          })
          .configureLogging(signalR.LogLevel.Information)
          .build();
    
        const startConnection = async () => {
          try {
            await connection.start();
            console.log('SignalR connected');
          } catch (err) {
            console.error('Connection failed:', err);
            setTimeout(startConnection, 5000);
          }
        };
    
        startConnection();
    
        connection.on('CountryUpdated', ({ code3, visited }) => {
          updateMap(code3, visited);
        });
    
        connection.on('NotifyTargetPrice', ({ notifyTargetPrice }) => {
            setFoundTargetPrice(notifyTargetPrice);
        });
    
        return () => {
          connection.stop()
            .then(() => console.log('SignalR connection stopped'))
            .catch(err => console.error('Error stopping connection:', err));
        };
      }, []);

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
    
            setCountriesVisited(updatedCountries.length);
    
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