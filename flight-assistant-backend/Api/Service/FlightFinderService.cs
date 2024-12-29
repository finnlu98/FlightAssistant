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

    public async Task<bool> GetFlightData()
    {
        try
        {
            await DeleteOldFlights();

            List<FlightQueryParse> queries = await QueryBuilder();

            foreach (var query in queries)
            {
                try
                {
                    string jsonContent = await GetFlights(query.SearchUrl);
                    //string jsonContent = await GetMockData();
                    var parsedFlights = await ParseFlights(jsonContent, query.TargetPrice);
                    
                    await _dbContext.Flights.AddRangeAsync(parsedFlights.Select(f => f.Flight));

                    await _dbContext.Layovers.AddRangeAsync(parsedFlights.Where(f => f.Layovers != null).SelectMany(f => f.Layovers!));
                   
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing query {query.SearchUrl}: {ex.Message}");
                }

                await Task.Delay(2000);
            }

            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Request error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An unexpected error occurred: {ex.Message}");
        }

        return false;
    }

    private async Task<List<ParsedFlight>> ParseFlights(string jsonContent, float targetPrice)
    {

        Search search = JsonSerializer.Deserialize<Search>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        string? searchUrl = search.search_metadata?.google_flights_url;

        PriceInsights priceInsights = search.price_insights;

        List<BestFlight>? bestFlights = search?.best_flights;
        List<OtherFlight>? otherFlights = search?.other_flights;

        List<ParsedFlight> flights = [];

        if(searchUrl != null) {
            if(bestFlights != null && bestFlights.Count > 1 ) {
                flights.AddRange(await ParseFlightType(bestFlights.Cast<FlightBase>().ToList(), targetPrice, searchUrl, priceInsights));
            }

            if(otherFlights != null && otherFlights.Count > 1 && bestFlights == null) {
                flights.AddRange(await ParseFlightType(otherFlights.Cast<FlightBase>().ToList(), targetPrice, searchUrl, priceInsights));
            }
        }

        if(flights.Count < 1) {
            _logger.LogInformation("No flights found");
        }

        return flights;
    }


        private async Task<List<ParsedFlight>> ParseFlightType(List<FlightBase> foundFlights, float targetPrice, string searchUrl, PriceInsights priceInsights) {
            List<ParsedFlight> parsedFlights = [];

            foreach (var foundFlight in foundFlights)
            {
                var flightDetail = foundFlight.flights?.FirstOrDefault();

                if(flightDetail == null) {
                    _logger.LogWarning("No flights found in the JSON data.");
                    return parsedFlights;
                }

                var parsedFlight = new ParsedFlight {
                    Flight =  new Flight {
                        DepartureAirport = flightDetail.departure_airport?.id ?? "Unknown",
                        ArrivalAirport = flightDetail.arrival_airport?.id ?? "Unknown",
                        DepartureTime = DateTime.TryParse(flightDetail.departure_airport?.time, out var departureTime) ? departureTime : DateTime.MinValue,
                        ArrivalTime = DateTime.TryParse(flightDetail.arrival_airport?.time, out var arrivalTime) ? arrivalTime : DateTime.MinValue,
                        Price = foundFlight.price,
                        TotalDuration = foundFlight.total_duration,
                        SearchUrl = searchUrl,
                        NumberLayovers = 0,
                        LayoverDuration = 0,
                        HasTargetPrice = await EvaluatePrice(foundFlight.price, targetPrice),
                        CreatedAt = DateTime.Now,
                        PriceRange = ClassifyPrice(foundFlight.price, priceInsights)
                    
                    }
                }; 

                var layovers = foundFlight.flights?.Count > 1;

                if(layovers) {
                    flightDetail = foundFlight.flights?.LastOrDefault();

                    if(flightDetail != null && foundFlight.layovers != null) {
                        parsedFlight.Flight.ArrivalAirport = flightDetail.arrival_airport.id;
                        parsedFlight.Flight.ArrivalTime = DateTime.TryParse(flightDetail.arrival_airport?.time, out var endArrivalTime) ? endArrivalTime : DateTime.MinValue;
                        parsedFlight.Flight.LayoverDuration = foundFlight.layovers?.Sum(l => l.duration) ?? 0; 
                        parsedFlight.Flight.NumberLayovers = foundFlight.layovers?.Count ?? 0;

                        parsedFlight.Layovers = ParseLayovers(foundFlight, parsedFlight.Flight);
                    }
                    
                }
                parsedFlights.Add(parsedFlight);
            }
                
            return parsedFlights;
        }

    private List<Layover> ParseLayovers(FlightBase foundFlight, Flight flight) {
        
        List<Layover> parsedLayovers = [];

        if(foundFlight.layovers != null) {
            foreach (var foundLayover in foundFlight.layovers)
            {
                var layover = new Layover {
                    Flight = flight,
                    Airport = foundLayover.id,
                    Name = foundLayover.name,
                    Duration = foundLayover.duration
                };

                parsedLayovers.Add(layover);
            }
        }
        return parsedLayovers;
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
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Api/Service/mockDataOther.json");

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
