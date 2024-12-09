import React from 'react';
import './App.css';
import MainCard from './Components/MainCard/MainCard';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import PersonalData from './Components/PersonalData/PersonalData';
import TravelDestinations from './Components/PersonalData/TravelDestinations/TravelDestinations';

function App() {

  var countryIds = ["FRA", "NOR", "RUS", "USA", "GER", "AUS", "CAN"]


  return (
    <div className="App">
      <Router>
        <Routes>
          <Route path='/' element={<MainCard countryIds={countryIds} />} />
          <Route path='/configure' element={<PersonalData countryIds={countryIds}/>}/>
          <Route path='/destination' element={<TravelDestinations countryIds={countryIds}/>}/>
        </Routes>
      </Router>
      
    </div>
  );
}

export default App;
