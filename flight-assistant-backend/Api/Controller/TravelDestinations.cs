using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Data.Models;
using flight_assistant_backend.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace flight_assistant_backend.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelDestinationsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public TravelDestinationsController(ApplicationDbContext context)
        {
            _context = context;
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

            return CreatedAtAction(nameof(CreateTravelDestination), new { code3 = newDestination.Code3, travelDate = newDestination.TravelDate });
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] TravelDestionation destination)
        {
            var travelDestination = _context.TravelDestinations.FirstOrDefault(c => c.Code3 == destination.Code3 && c.TravelDate == destination.TravelDate);


            if(travelDestination == null) {
                return Conflict($"A travel destination with code '{destination.Code3}' and date '{destination.TravelDate}' not found.");
            }

            _context.TravelDestinations.Remove(travelDestination);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
