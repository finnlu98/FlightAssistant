import './App.css';
import MainCard from './Components/Map/MainCard';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import VisitCountries from './Components/PersonalData/VisitCountries/VisitCountries';
import TravelDestinations from './Components/PersonalData/TravelDestinations/TravelDestinations';
import LandingPage from './Components/PersonalData/LandingPage';
import Flights from './Components/PersonalData/FlightFinder/Flights/Flights';

function App() {
  return (
    <div className="App">
      <Router>
        <Routes>
          <Route path='/' element={<MainCard />} />
          <Route path='/home' element={<LandingPage/>} />
          <Route path='/countries' element={<VisitCountries/>}/>
          <Route path='/destination' element={<TravelDestinations/>}/>
          <Route path='/flights' element={<Flights/>}/>
        </Routes>
      </Router>
    </div>
  );
}

export default App;
