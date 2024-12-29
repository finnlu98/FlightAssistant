import { Layover } from "./Layover"

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
    createdAt: Date
    priceRange: PriceRange
    layovers: Layover[]
}

export enum PriceRange {
    High = 0,
    Normal = 1,
    Low = 2
}