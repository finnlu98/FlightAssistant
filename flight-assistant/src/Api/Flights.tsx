import axios from "axios";
import { Flight } from "../Models/Flight";
import { UrlBuilder } from "./UrlBuilder";

const API_BASE_URL = `${UrlBuilder.getBaseUrl()}/flights`;

const FlightService = {
  getFlights: async (): Promise<Flight[]> => {
    try {
      const response = await axios.get<Flight[]>(API_BASE_URL);
      return response.data;
    } catch (error) {
      console.error('Error fetching flights:', error);
      throw error;
    }
  },

  notifyReadFlights: async (): Promise<void> => {
    try {
      await axios.get(`${API_BASE_URL}/readflights`);
    } catch (error) {
      console.error('Error notifying read flights:', error);
      throw error;
    }
  }


};

export default FlightService;