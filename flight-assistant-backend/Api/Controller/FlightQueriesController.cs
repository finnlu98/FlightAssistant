using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Api.Service;
using flight_assistant_backend.Data.Models;
using Microsoft.AspNetCore.Mvc;


namespace flight_assistant_backend.Api.Controller;

    [ApiController]
    [Route("api/[controller]")]
    public class FlightQueriesController : ControllerBase
    {

        private readonly FlightFinderService _flightFinderService;

        private readonly ApplicationDbContext _context;

        public FlightQueriesController(ApplicationDbContext context, FlightFinderService flightFinderService)
        {
            _context = context;
            _flightFinderService = flightFinderService;
        }

        [HttpGet]
        public IEnumerable<FlightQuery> Get()
        {
            return [.. _context.FlightQueries];
        }

        [HttpPost]
        public IActionResult Post([FromBody] FlightQuery newQuery)
        {
            if (_context.FlightQueries.Any(c =>
                c.DepartureAirport == newQuery.DepartureAirport &&
                c.ArrivalAirport == newQuery.ArrivalAirport &&
                c.DepartureTime == newQuery.DepartureTime &&
                c.ReturnTime == newQuery.ReturnTime))
            {
                return Conflict($"Flight query for added query already exists.");
            }

            _context.FlightQueries.Add(newQuery);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Get), newQuery);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var query = _context.FlightQueries.FirstOrDefault(c => c.Id == id);

            if (query == null)
            {
                return NotFound($"Query with Id '{id}' not found.");
            }

            _context.FlightQueries.Remove(query);
            _context.SaveChanges();

            return NoContent();
        }

}