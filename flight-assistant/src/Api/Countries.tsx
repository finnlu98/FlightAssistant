import axios from 'axios';
import { Country } from '../Models/Country';

const API_BASE_URL = 'http://192.168.50.154:5208/api/countries';

const CountryService = {
  getCountries: async (): Promise<Country[]> => {
    try {
      const response = await axios.get<Country[]>(API_BASE_URL);
      return response.data;
    } catch (error) {
      console.error('Error fetching countries:', error);
      throw error;
    }
  },

  addCountry: async (newCountry: Country): Promise<Country> => {
    try {
      const response = await axios.post<Country>(API_BASE_URL, newCountry);
      return response.data;
    } catch (error) {
      console.error('Error adding country:', error);
      throw error;
    }
  },

  setVisitedCountry: async (code3: string, visited: boolean): Promise<string> => {
    try {
        const response = await axios.put(`${API_BASE_URL}/${code3}`, visited, {
            headers: {
              'Content-Type': 'application/json',
            },
          });
      return response.data;
    } catch (error) {
      console.error('Error adding country:', error);
      throw error;
    }
  },

};

export default CountryService;
