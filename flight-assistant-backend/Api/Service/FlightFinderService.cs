using System.Text.Json;
using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Data.Models;
using flight_assistant_backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace flight_assistant_backend.Api.Service;

public class FlightFinderService {
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FlightFinderService> _logger;

    private readonly IHubContext<MapHub> _hubContext;

        

    public FlightFinderService(HttpClient httpClient, ApplicationDbContext dbContext, ILogger<FlightFinderService> logger, IHubContext<MapHub> hubContext)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task<object> GetFlightData()
    {
        try
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Api/Service/mockData.json");

            if (!File.Exists(filePath))
            {
                _logger.LogError("Mock data file not found.");
                return null;
            }

            var jsonContent = await File.ReadAllTextAsync(filePath);

            List<Flight> flights = await ParseBestFlights(jsonContent, 2600);

            await _dbContext.Flights.AddRangeAsync(flights);

            await _dbContext.SaveChangesAsync();

            return flights;

        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Request error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
            return null;
        }
    }

    private async Task<List<Flight>> ParseBestFlights(string jsonContent, int targetPrice)
    {

        Search search = JsonSerializer.Deserialize<Search>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        var searchUrl = search.search_metadata?.google_flights_url;

        List<BestFlight> bestFlights = search.best_flights;

        List<Flight> flights = [];

        if (bestFlights == null || bestFlights.Count == 0 || searchUrl == null)
        {
            _logger.LogWarning("No best flights found in the JSON data.");
            return flights;
        }

        foreach (var bestFlight in bestFlights)
        {
            var flightDetail = bestFlight.flights?.FirstOrDefault();

            if(flightDetail == null) {
                _logger.LogWarning("No best flights found in the JSON data.");
                return flights;
            }

            var flight = new Flight {
                DepartureAirport = flightDetail.departure_airport?.id ?? "Unknown",
                ArrivalAirport = flightDetail.arrival_airport?.id ?? "Unknown",
                DepartureTime = DateTime.TryParse(flightDetail.departure_airport?.time, out var departureTime) ? departureTime : DateTime.MinValue,
                ArrivalTime = DateTime.TryParse(flightDetail.arrival_airport?.time, out var arrivalTime) ? arrivalTime : DateTime.MinValue,
                Price = bestFlight.price,
                TotalDuration = bestFlight.total_duration,
                SearchUrl = searchUrl,
                NumberLayovers = 0,
                LayoverDuration = 0,
                HasTargetPrice = await EvaluatePrice(bestFlight.price, targetPrice)
            };
            
            var layovers = bestFlight.flights?.Count > 1;

            if(layovers) {
                flightDetail = bestFlight.flights?.LastOrDefault();

                if(flightDetail != null && bestFlight.layovers != null) {
                    flight.ArrivalAirport = flightDetail.arrival_airport.id;
                    flight.ArrivalTime = DateTime.TryParse(flightDetail.arrival_airport?.time, out var endArrivalTime) ? endArrivalTime : DateTime.MinValue;
                    flight.LayoverDuration = bestFlight.layovers?.Sum(l => l.duration) ?? 0; 
                    flight.NumberLayovers = bestFlight.layovers?.Count ?? 0;
                }
                
            }
            
            flights.Add(flight);
        }
            
        return flights;
    }

    public async Task<List<string>> QueryBuilder() {
        var queries = await _dbContext.FlightQueries.ToListAsync();

        var API_KEY = Environment.GetEnvironmentVariable("API_KEY");

        if (string.IsNullOrEmpty(API_KEY))
        {
            throw new InvalidOperationException("API key is not set in the environment variables.");
        }

        List<string> stringQueries = [];        

        if (queries == null || queries.Count == 0)
        {
            return stringQueries;
        }   

        foreach (var flightQuery in queries)
        {
            
            var departureAirport = flightQuery.DepartureAirport;
            var arrivalAirport = flightQuery.ArrivalAirport;
            var departureDate = flightQuery.DepartureTime.ToString("yyyy-MM-dd");
            var returnDate = flightQuery.ReturnTime.ToString("yyyy-MM-dd");

            var stringQuery = $"https://serpapi.com/search.json?engine=google_flights&departure_id={departureAirport}&arrival_id={arrivalAirport}&gl=us&hl=en&currency=NOK&outbound_date={departureDate}&return_date={returnDate}&deep_search=true&api_key={API_KEY}"; 
            
            stringQueries.Add(stringQuery);
        }

        return stringQueries;
    }


    public void GetFlights(string query) {

    }


    public async Task<bool> EvaluatePrice(int price, int targetPrice) {
        
        if(price <= targetPrice) {
            await _hubContext.Clients.All.SendAsync("NotifyTargetPrice", new { notifyTargetPrice = true });
            
            return true;
        }
        
        return false;
    }
}
