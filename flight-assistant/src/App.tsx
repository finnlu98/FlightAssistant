import React from 'react';
import './App.css';
import MainCard from './Components/Map/MainCard';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import VisitCountries from './Components/PersonalData/VisitCountries/VisitCountries';
import TravelDestinations from './Components/PersonalData/TravelDestinations/TravelDestinations';
import LandingPage from './Components/PersonalData/LandingPage';

function App() {

  var countryIds = ["FRA", "NOR", "RUS", "USA", "GER", "AUS", "CAN"]


  return (
    <div className="App">
      <Router>
        <Routes>
          <Route path='/' element={<MainCard countryIds={countryIds} />} />
          <Route path='/home' element={<LandingPage/>} />
          <Route path='/countries' element={<VisitCountries/>}/>
          <Route path='/destination' element={<TravelDestinations/>}/>
        </Routes>
      </Router>
      
    </div>
  );
}

export default App;
