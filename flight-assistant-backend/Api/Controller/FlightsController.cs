using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Api.Service;
using flight_assistant_backend.Data.Models;
using flight_assistant_backend.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


namespace flight_assistant_backend.Api.Controller;

    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {


        private readonly ApplicationDbContext _context;

        private readonly IHubContext<MapHub> _hubContext;


        public FlightsController(ApplicationDbContext context, IHubContext<MapHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IEnumerable<Flight> Get()
        {
            return [.. _context.Flights];
        }

        [HttpGet("readFlights")]
        public async Task<IActionResult> GetReadFlights()
        {
            await _hubContext.Clients.All.SendAsync("NotifyTargetPrice", new { notifyTargetPrice = false });

            return Ok("Flight table read notification sent.");
        }
}