import React, { useEffect, useState } from 'react';
import moment, { Moment } from 'moment';
import TravelDestinationService from '../../../Api/TravelDestinations';
import MapService from '../../../Api/MapService';

interface NextTravelDestination {
    mapService: React.MutableRefObject<MapService>;
}

const NextTravelDestination: React.FC<NextTravelDestination> = ({mapService}) => {

    const [nextDestination, setNextDestionation] = useState<string>("");
    const [nextDateTravel, setNextDateTravel] = useState<Moment>(moment());

    const diffInDays = nextDateTravel.diff(moment(), 'days');
    const timeToDisplay = `${diffInDays} day${diffInDays > 1 ? 's' : ''}`;

    useEffect(() => {
        fetchTravelDestinations();
        mapService.current.onNotifyPlannedTrip(setPlannedTrip);
      }, []);

    const fetchTravelDestinations = async () => {
        try {
          const travelDestinations = await TravelDestinationService.getTravelDestinations();    
          
          if (travelDestinations.length === 0) {
              return;
          }
          
          const upcomingDestinations = travelDestinations.filter(destination => moment(destination.travelDate).isAfter(moment()));
          
          if (upcomingDestinations.length === 0) {
              return;
          }
          
          const closestDestination = upcomingDestinations.reduce((closest, destination) => 
              moment(destination.travelDate).diff(moment(), 'days') < moment(closest.travelDate).diff(moment(), 'days') 
              ? destination 
              : closest
          );
          
          setPlannedTrip(closestDestination.country.name, closestDestination.travelDate);

        } catch (err) {
          console.error('Error fetching visited countries:', err);
        }
      };

    const setPlannedTrip = (nextDestination : string, nextDate : Date) => {
        setNextDestionation(nextDestination);
        setNextDateTravel(moment(nextDate, "YYYY/MM/DD"));
    }

    return (
        <div>
            {nextDestination !== "" ? (
                <h5>
                {timeToDisplay} to {nextDestination}..
                </h5>
            ) : (
                <p>You need to plan a trip gurl...</p>
            )}
        </div>
    );
};


export default NextTravelDestination;