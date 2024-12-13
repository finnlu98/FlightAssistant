public class BestFlights {
    public required List<BestFlight>  best_flights {get; set;}
}


public class BestFlight
{
    public required List<FlightDetail> flights { get; set; }
    public int price { get; set; }
}

public class FlightDetail
{
    public required AirportInfo departure_airport { get; set; }
    public required AirportInfo arrival_airport { get; set; }
}

public class AirportInfo
{
    public required string name { get; set; }
    public required string id { get; set; }
    public required string time { get; set; }
}
