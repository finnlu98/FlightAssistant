using System.Text.Json;
using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Api.Settings;
using flight_assistant_backend.Data.Models;
using flight_assistant_backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace flight_assistant_backend.Api.Service;

public class FlightFinderService {

    private readonly QuerySettings _querySettings;

    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FlightFinderService> _logger;

    private readonly IHubContext<MapHub> _hubContext;

        

    public FlightFinderService(
        HttpClient httpClient, 
        ApplicationDbContext dbContext, 
        ILogger<FlightFinderService> logger, 
        IHubContext<MapHub> hubContext, 
        IOptions<QuerySettings> querySettings)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _logger = logger;
        _hubContext = hubContext;
        _querySettings = querySettings.Value;
    }

    public async Task<List<Flight>> GetFlightData()
    {
        try
        {
            await DeleteOldFlights();

            // string jsonContent = await getMockData();

            List<FlightQueryParse> quieres = await QueryBuilder();

            List<Flight> flights = [];
            foreach (var query in quieres)
            {
                string jsonContent = await GetFlights(query.SearchUrl);

                flights.AddRange(await ParseBestFlights(jsonContent, query.TargetPrice));
            }

            await _dbContext.Flights.AddRangeAsync(flights);

            await _dbContext.SaveChangesAsync();

            return flights;

        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Request error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
        }

        return [];
    }

    private async Task<List<Flight>> ParseBestFlights(string jsonContent, float targetPrice)
    {

        Search search = JsonSerializer.Deserialize<Search>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        var searchUrl = search.search_metadata?.google_flights_url;

        PriceInsights priceInsights = search.price_insights;

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
                HasTargetPrice = await EvaluatePrice(bestFlight.price, targetPrice),
                CreatedAt = DateTime.Now,
                PriceRange = ClassifyPrice(bestFlight.price, priceInsights)
                
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

    public async Task<List<FlightQueryParse>> QueryBuilder() {
        var queries = await _dbContext.FlightQueries.ToListAsync();

        var API_KEY = _querySettings.ApiKey;

        _logger.LogInformation(API_KEY);

        if (string.IsNullOrEmpty(API_KEY))
        {
            throw new InvalidOperationException("API key is not set in the environment variables.");
        }

        List<FlightQueryParse> stringQueries = [];        

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
            
            stringQueries.Add( new FlightQueryParse {
                SearchUrl = stringQuery,
                TargetPrice = flightQuery.TargetPrice
            } );
        }

        return stringQueries;
    }


    public async Task<string> GetFlights(string query) {
        try
    {
        _logger.LogInformation($"Fetching flight data from {query}");

        HttpResponseMessage response = await _httpClient.GetAsync(query);

        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        
        _logger.LogInformation("Successfully fetched flight data.");

        return responseBody;
    }
    catch (HttpRequestException e)
    {
        _logger.LogError(e, "Error occurred while fetching flight data from {ApiUrl}", query);
        throw;
    }
    }

    public async Task<string> GetMockData() {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Api/Service/mockData.json");

            if (!File.Exists(filePath))
            {
                _logger.LogError("Mock data file not found.");
            }

        var jsonContent = await File.ReadAllTextAsync(filePath);
        
        return jsonContent;
    }


    public async Task<bool> EvaluatePrice(int price, float targetPrice) {
        
        if(price <= targetPrice) {
            await _hubContext.Clients.All.SendAsync("NotifyTargetPrice", new { notifyTargetPrice = true });
            
            return true;
        }
        
        return false;
    }

    public PriceRange ClassifyPrice(int price, PriceInsights priceInsights) {
        
        List<int> priceRange = [.. priceInsights.typical_price_range];

        int highestPrice = priceRange.Max();
        int lowestPrice = priceRange.Min();


        if (priceRange == null || priceRange.Count <= 1)
        {
            throw new ArgumentException("Price list cannot be null or must contain at least two prices.");
        }

        double range = highestPrice - lowestPrice;

        double lowThreshold = lowestPrice + 0.25 * range;

        double highThreshold = lowestPrice + 0.75 * range;

        if (price <= lowThreshold)
        {
            return PriceRange.Low;
        }
        else if (price >= highThreshold)
        {
            return PriceRange.High;
        }
        else
        {
            return PriceRange.Normal;
        }
    }
    

    public async Task DeleteOldFlights()
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-_querySettings.deleteNOld);

        var oldFlights = _dbContext.Flights
                                .Where(f => f.CreatedAt < cutoffDate)
                                .ToList(); 

        if (oldFlights.Count == 0)
        {
            _logger.LogInformation($"No queries older than {_querySettings.deleteNOld} days");
            return;
        }

        _dbContext.Flights.RemoveRange(oldFlights);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Removed {oldFlights.Count} flight queries older than 15 days.");
    }
}
