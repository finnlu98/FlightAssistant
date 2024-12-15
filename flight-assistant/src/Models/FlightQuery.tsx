export interface FlightQuery {
    id: string
    departureAirport: string
    arrivalAirport: string
    departureTime: Date
    returnTime: Date
    targetPrice: number

}