export interface Flight {
    id: string
    departureAirport: string
    arrivalAirport: string
    departureTime: Date
    arrivalTime: Date
    price: number
    totalDuration: number
    numberLayovers: number
    layoverDuration: number
    searchUrl: string
    hasTargetPrice : boolean

}