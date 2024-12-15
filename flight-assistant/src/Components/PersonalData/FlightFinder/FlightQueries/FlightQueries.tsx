import React, { useEffect, useState } from 'react';
import FlightQueryService from '../../../../Api/FlightQueries';
import { FlightQuery } from '../../../../Models/FlightQuery';
import moment from 'moment';
import './FlightQueries.css';
import { RiDeleteBin5Line } from "react-icons/ri";
import { FaRegPlusSquare } from "react-icons/fa";






interface FlightQueries {
    
}

const FlightQueries: React.FC = () => {

    const [flightQueries, setFlightQueries] = useState<FlightQuery[]>([]);
    const [flightQuery, setFlightQuery] = useState<FlightQuery>({
        id: '',
        departureAirport: '',
        arrivalAirport: '',
        departureTime: new Date(),
        returnTime: new Date(),
        targetPrice: 0,
    });

    useEffect(() => {
        const fetchFlightQueries = async () => {
            try {
                const storedFlightQueries = await FlightQueryService.getFlightQueries();
                setFlightQueries(storedFlightQueries);
            } catch (err) {
                console.error('Error fetching flight queries:', err);
            }
        };

        fetchFlightQueries();
    }, []);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;

        setFlightQuery((prev) => ({
            ...prev,
            [name]:
                name === 'departureTime' || name === 'arrivalTime'
                    ? new Date(value)
                    : name === 'targetPrice'
                    ? parseFloat(value) || 0
                    : value,
        }));
    };

    const handleAddFlightQuery = async () => {
        if (flightQuery.departureAirport && flightQuery.arrivalAirport) {
            
            try {
                var createdFlightQuery : FlightQuery = await FlightQueryService.addFlightQuery(flightQuery);

                setFlightQueries((prev) => [
                    ...prev,
                    { ...createdFlightQuery },
                ]);
                setFlightQuery({
                    id: '',
                    departureAirport: '',
                    arrivalAirport: '',
                    departureTime: new Date(),
                    returnTime: new Date(),
                    targetPrice: 0,
                });
            } catch (error) {
                console.error("Error adding query", error);
            }
        }
    };

    const handleRemoveFlightQuery = async (id: string) => {
        try {
            await FlightQueryService.deleteFlightQuery(id);
            
            setFlightQueries((prev) => prev.filter((query) => query.id !== id));

        } catch (error) {
            console.error("Error deleting query", error)
        }
        
    };



    return (
        <div>
            <h3>Edit Queries</h3>
            <div className="center-container">
                <ul className="query-list">
                    <li key="input" className="query-list-item">
                    <input
                    type="text"
                    name="departureAirport"
                    placeholder="Departure airport"
                    className="query-input"
                    value={flightQuery.departureAirport}
                    onChange={handleChange}
                />
                <input
                    type="text"
                    name="arrivalAirport"
                    placeholder="Arrival airport"
                    className="query-input"
                    value={flightQuery.arrivalAirport}
                    onChange={handleChange}
                />
                <input
                    type="date"
                    name="departureTime"
                    className="query-input"
                    value={flightQuery.departureTime.toISOString().split('T')[0]}
                    onChange={handleChange}
                />
                <input
                    type="date"
                    name="arrivalTime"
                    className="query-input"
                    value={flightQuery.returnTime.toISOString().split('T')[0]}
                    onChange={handleChange}
                />
                <input
                    type="text"
                    name="targetPrice"
                    placeholder="Target price"
                    className="query-input"
                    value={flightQuery.targetPrice}
                    onChange={handleChange}
                />
                <p className="add-icon-container" onClick={handleAddFlightQuery}>
                    <FaRegPlusSquare size={20} className="add-icon" />
                </p>
                    </li>
                    <li key="header" className="query-list-item header">
                        <p>Departure</p>
                        <p>Arrival</p>
                        <p>Departure Date</p>
                        <p>Return Date</p>
                        <p>Target Price</p>
                        <p></p>
                    </li>
                    {flightQueries.map((query) => (
                        <li key={query.id} className="query-list-item row">
                            <p>{query.departureAirport}</p>
                            <p>{query.arrivalAirport}</p>
                            <p>{moment(query.departureTime).format('YYYY-MM-DD')}</p>
                            <p>{moment(query.returnTime).format('YYYY-MM-DD')}</p>
                            <p>{query.targetPrice.toFixed(0)} NOK</p>
                            <p><div ><RiDeleteBin5Line className='delete-icon' onClick={() => handleRemoveFlightQuery(query.id)}/> </div></p>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};


export default FlightQueries;