using System.Text.Json;
using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace flight_assistant_backend.Api.Service;

public class FlightFinderService {
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FlightFinderService> _logger;

    public FlightFinderService(HttpClient httpClient, ApplicationDbContext dbContext, ILogger<FlightFinderService> logger)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _logger = logger;
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

            List<Flight> flights = ParseBestFlights(jsonContent);

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


    private List<Flight> ParseBestFlights(string jsonContent)
    {
        BestFlights bestFlights = JsonSerializer.Deserialize<BestFlights>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        List<Flight> flights = [];

        if (bestFlights?.best_flights == null || bestFlights.best_flights.Count == 0)
        {
            _logger.LogWarning("No best flights found in the JSON data.");
            return flights;
        }

        foreach (var bestFlight in bestFlights.best_flights)
        {
            var flightDetail = bestFlight.flights?.FirstOrDefault();

            if (flightDetail == null)
            {
                _logger.LogWarning("No flight details found in one of the best flights entries.");
                continue;
            }

            flights.Add(new Flight
            {
                DepartureAirport = flightDetail.departure_airport?.id ?? "Unknown",
                ArrivalAirport = flightDetail.arrival_airport?.id ?? "Unknown",
                DepartureTime = DateTime.TryParse(flightDetail.departure_airport?.time, out var departureTime) ? departureTime : DateTime.MinValue,
                ArrivalTime = DateTime.TryParse(flightDetail.arrival_airport?.time, out var arrivalTime) ? arrivalTime : DateTime.MinValue,
                Price = bestFlight.price
            });
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
}
