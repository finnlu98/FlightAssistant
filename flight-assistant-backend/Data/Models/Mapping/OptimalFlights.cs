public class Search {
    public required SearchMetadata search_metadata {get; set;}
    public required List<BestFlight>  best_flights {get; set;}
}

public class SearchMetadata {
    public required string google_flights_url  {get; set;}
}





public class BestFlight
{
    public required List<FlightDetail> flights { get; set; }
    public int price { get; set; }
    public int total_duration { get; set; }

    public List<Layovers>? layovers { get; set; }
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

public class Layovers {
    public required int duration { get; set; }
}