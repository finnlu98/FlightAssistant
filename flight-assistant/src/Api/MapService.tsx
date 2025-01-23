import * as signalR from '@microsoft/signalr';
import { UrlBuilder } from './UrlBuilder';

const MAP_CONNECTION_URL = `${UrlBuilder.getMapUrl()}`;

const MAP_CONNECTION_URL = `${process.env.REACT_APP_SERVER_MAP_URL}`;

export class MapService {
    private connection: signalR.HubConnection;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(MAP_CONNECTION_URL, {
        withCredentials: true,
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.initializeConnection();
  }

  async initializeConnection() {
    try {
      await this.connection.start();
      console.log('SignalR connected');
    } catch (err) {
      console.error('Connection failed:', err);
      setTimeout(() => this.initializeConnection(), 5000);
    }
  }

  onCountryUpdated(updateMap: (code3: string, visited: boolean) => void) {
    this.connection.on('CountryUpdated', ({ code3, visited }) => {
        updateMap(code3, visited);
      });
  }

  onNotifyTargetPrice(setFoundTargetPrice: React.Dispatch<React.SetStateAction<boolean>>) {
    this.connection.on('NotifyTargetPrice', ({ notifyTargetPrice }) => {
        setFoundTargetPrice(notifyTargetPrice);
    });
  }

  onNotifyPlannedTrip(setPlannedTrip: (nextDestination :string, nextTravelDate : Date) => void) {
    this.connection.on('NotifyPlannedTrip', ({ nextDestination, nextDate }) => {
      if (nextDestination?.trim()) {
        setPlannedTrip(nextDestination, nextDate);
      } else {
        setPlannedTrip("", nextDate);
      }
      
      
    });
  }


  async stopConnection() {
    try {
      await this.connection.stop();
      console.log('SignalR connection stopped');
    } catch (err) {
      console.error('Error stopping connection:', err);
    }
  }
}

export default MapService;
