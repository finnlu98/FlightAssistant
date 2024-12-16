import React, { useEffect, useState } from 'react';
import moment, { Moment } from 'moment';
import TravelDestinationService from '../../../Api/TravelDestinations';



interface NextTravelDestination {
    
}

const NextTravelDestination: React.FC<NextTravelDestination> = () => {

    const [nextDestination, setNextDestionation] = useState<string>("");
    const [nextDateTravel, setNextDateTravel] = useState<Moment>(moment());

    useEffect(() => {
        const fetchTravelDestinations = async () => {
          try {
            const travelDestinations = await TravelDestinationService.getTravelDestinations();    
            
            if(travelDestinations.length === 0) {
                return;
            }
            
            const closestDestination = travelDestinations.reduce((closest, destination) => 
                moment(destination.travelDate).diff(moment(), 'days') < moment(closest.travelDate).diff(moment(), 'days') ? destination : closest
            );

            setNextDestionation(closestDestination.country.name);
            setNextDateTravel(moment(closestDestination.travelDate, "YYYY/MM/DD"));

          } catch (err) {
            console.error('Error fetching visited countries:', err);
          }
        };
    
        fetchTravelDestinations();
      }, []);

    const currentDate = moment();

    const diffInDays = nextDateTravel.diff(currentDate, 'days');
    const diffInWeeks = nextDateTravel.diff(currentDate, 'weeks');
    const diffInMonths = nextDateTravel.diff(currentDate, 'months');

    let timeToDisplay;

    if (diffInDays === 0) {
        timeToDisplay = "";
    } else if (diffInMonths >= 1) {
        timeToDisplay = `${diffInMonths} month${diffInMonths > 1 ? 's' : ''}`;
    } else if (diffInWeeks >= 2) {
        timeToDisplay = `${diffInWeeks} week${diffInWeeks > 1 ? 's' : ''}`;
    } else {
        timeToDisplay = `${diffInDays} day${diffInDays > 1 ? 's' : ''}`;
    }

    return (
        <div>
            {nextDestination !== "" ? (
                <h5>
                {timeToDisplay} to {nextDestination}..
                </h5>
            ) : (
                <p>No trips planned, you need to plan a trip gurl...</p>
            )}
        </div>
    );
};


export default NextTravelDestination;