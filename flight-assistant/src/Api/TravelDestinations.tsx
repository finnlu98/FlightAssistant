import axios from "axios";
import { TravelDestination } from "../Models/TravelDestination";

const API_BASE_URL = `${process.env.REACT_APP_SERVER_API_URL}/traveldestinations`;



const TravelDestinationService = {
  getTravelDestinations: async (): Promise<TravelDestination[]> => {
    try {
      const response = await axios.get<TravelDestination[]>(API_BASE_URL);
      return response.data;
    } catch (error) {
      console.error('Error fetching travel destinations:', error);
      throw error;
    }
  },

  addTravelDestination: async (code3: string, travelDate: string): Promise<TravelDestination> => {
    try {
      const response = await axios.post<TravelDestination>(API_BASE_URL, {Code3: code3, TravelDate: travelDate});
      return response.data;
    } catch (error) {
      console.error('Error adding travel destination:', error);
      throw error;
    }
  },

  deleteTravelDestination: async (code3: string, travelDate: string): Promise<TravelDestination> => {
    try {
        const response = await axios.delete<TravelDestination>(API_BASE_URL, {
            data: {
                Code3: code3,
                TravelDate: travelDate
            }
        });
        return response.data;
    } catch (error) {
      console.error('Error adding country:', error);
      throw error;
    }
  },

};

export default TravelDestinationService;
