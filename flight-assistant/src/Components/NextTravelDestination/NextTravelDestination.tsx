import React from 'react';
import moment from 'moment';



interface NextTravelDestination {
    
}

const NextTravelDestination: React.FC<NextTravelDestination> = () => {

    const nextDestination = "Maldives";
    const dateLeaving = "14/12/2024";

    // Parse the date and calculate the difference
    const travelDate = moment(dateLeaving, "DD/MM/YYYY");
    const currentDate = moment();

    const diffInDays = travelDate.diff(currentDate, 'days');
    const diffInWeeks = travelDate.diff(currentDate, 'weeks');
    const diffInMonths = travelDate.diff(currentDate, 'months');

    let timeToDisplay;
    if (diffInMonths >= 1) {
        timeToDisplay = `${diffInMonths} month${diffInMonths > 1 ? 's' : ''}`;
    } else if (diffInWeeks >= 2) {
        timeToDisplay = `${diffInWeeks} week${diffInWeeks > 1 ? 's' : ''}`;
    } else {
        timeToDisplay = `${diffInDays} day${diffInDays > 1 ? 's' : ''}`;
    }

    return (
        <div>
            <h5>
                {nextDestination} coming up in {timeToDisplay}..
            </h5>
        </div>
    );
};


export default NextTravelDestination;