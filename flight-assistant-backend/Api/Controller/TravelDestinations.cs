using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Data.Models;
using flight_assistant_backend.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace flight_assistant_backend.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelDestinationsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MapHub> _hubContext;


        public TravelDestinationsController(ApplicationDbContext context, IHubContext<MapHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TravelDestionation>>> Get()
            {
                var travelDestinations = await _context.TravelDestinations
                .Include(td => td.Country)
                .ToListAsync();
                  
                return Ok(travelDestinations);
            }

        [HttpPost]
        public async Task<IActionResult> CreateTravelDestination([FromBody] TravelDestionation newDestination)
        {
                        
            if (_context.TravelDestinations.Any(c => c.Code3 == newDestination.Code3 && c.TravelDate == newDestination.TravelDate))
            {
                return Conflict($"A travel destination with code '{newDestination.Code3}' and date '{newDestination.TravelDate}' already exists.");
            }

            _context.TravelDestinations.Add(newDestination);
            await _context.SaveChangesAsync();

            await ClosestTripAsync();

            return CreatedAtAction(nameof(CreateTravelDestination), new { code3 = newDestination.Code3, travelDate = newDestination.TravelDate });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] TravelDestionation destination)
        {
            var travelDestination = _context.TravelDestinations.FirstOrDefault(c => c.Code3 == destination.Code3 && c.TravelDate == destination.TravelDate);


            if(travelDestination == null) {
                return Conflict($"A travel destination with code '{destination.Code3}' and date '{destination.TravelDate}' not found.");
            }

            _context.TravelDestinations.Remove(travelDestination);
            _context.SaveChanges();

            await ClosestTripAsync();

            return NoContent();
        }


        public async Task ClosestTripAsync()
        {
            var closestDestination = _context.TravelDestinations
                .OrderBy(c => Math.Abs((c.TravelDate - DateTime.Now).TotalDays))
                .FirstOrDefault();

            string nextDestination = "";
            DateOnly nextDate = default;

            if (closestDestination != null)
            {
                if (closestDestination.Country == null)
                {
                    await _context.Entry(closestDestination).Reference(c => c.Country).LoadAsync();
                }

                nextDestination = closestDestination.Country?.Name ?? "";
                nextDate = DateOnly.FromDateTime(closestDestination.TravelDate);
            }

            try
            {
                await _hubContext.Clients.All.SendAsync("NotifyPlannedTrip", new { nextDestination, nextDate });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error sending SignalR notification: {ex.Message}");
            }
        }

    }
}
