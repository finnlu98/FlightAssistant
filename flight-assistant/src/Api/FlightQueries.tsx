import axios from "axios";
import { FlightQuery } from "../Models/FlightQuery";
import moment from "moment";
import { UrlBuilder } from "./UrlBuilder";

const API_BASE_URL = `${UrlBuilder.getBaseUrl()}/flightqueries`;

const FlightQueryService = {
  getFlightQueries: async (): Promise<FlightQuery[]> => {
    try {
      const response = await axios.get<FlightQuery[]>(API_BASE_URL);
      return response.data;
    } catch (error) {
      console.error('Error fetching flight queries:', error);
      throw error;
    }
  },


  addFlightQuery: async (flightQuery : FlightQuery): Promise<FlightQuery> => {
    try {
      const response = await axios.post<FlightQuery>(API_BASE_URL,
        {
            DepartureAirport: flightQuery.departureAirport, 
            ArrivalAirport: flightQuery.arrivalAirport,
            DepartureTime: moment(flightQuery.departureTime).format("YYYY-MM-DD"),
            ReturnTime: moment(flightQuery.returnTime).format("YYYY-MM-DD"),
            TargetPrice: flightQuery.targetPrice
        }
      );
      return response.data;
    } catch (error) {
      console.error('Error adding Flight Query:', error);
      throw error;
    }
  },

  deleteFlightQuery : async (id: string): Promise<FlightQuery> => {
    try {
        const response = await axios.delete<FlightQuery>(`${API_BASE_URL}/${id}`);
        return response.data;
    } catch (error) {
      console.error('Error deleting Flight Query:', error);
      throw error;
    }
  },


};

export default FlightQueryService;