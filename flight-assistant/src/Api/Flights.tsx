import axios from "axios";
import { Flight } from "../Models/Flight";

const API_BASE_URL = `${process.env.REACT_APP_SERVER_API_URL}/flights`;

const FlightService = {
  getFlights: async (): Promise<Flight[]> => {
    try {
      const response = await axios.get<Flight[]>(API_BASE_URL);
      return response.data;
    } catch (error) {
      console.error('Error fetching flights:', error);
      throw error;
    }
  }


};

export default FlightService;