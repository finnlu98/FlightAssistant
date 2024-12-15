using flight_assistant_backend.Api.Data;
using flight_assistant_backend.Api.Service;
using flight_assistant_backend.Data.Models;
using Microsoft.AspNetCore.Mvc;


namespace flight_assistant_backend.Api.Controller;

    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {


        private readonly ApplicationDbContext _context;

        public FlightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Flight> Get()
        {
            return [.. _context.Flights];
        }

        

       

}