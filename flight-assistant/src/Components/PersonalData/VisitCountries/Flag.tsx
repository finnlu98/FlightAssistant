import React from 'react';
import FlagMap from '../FlagMap';

interface FlagProps {
    code2: string; // Country code (e.g., "US", "GB")
}

const Flag: React.FC<FlagProps> = ({ code2 }) => {
    const FlagComponent = FlagMap[code2];

    if (!FlagComponent) {
        return <span>Flag not available</span>;
    }

    return <FlagComponent />;
};

export default Flag;