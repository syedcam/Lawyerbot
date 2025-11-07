using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerDirectory.Data;
using LawyerDirectory.Models;

namespace LawyerDirectory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LawyersController : ControllerBase
    {
        private readonly LawyerContext _context;

        public LawyersController(LawyerContext context)
        {
            _context = context;
        }

        // GET: api/Lawyers/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Lawyer>>> SearchLawyers(
            [FromQuery] string? areaOfPractice,
            [FromQuery] int? minYears,
            [FromQuery] int? maxYears,
            [FromQuery] string? state,
            [FromQuery] string? city)
        {
            var query = _context.Lawyers.AsQueryable();

            // Filter by area of practice (case-insensitive, partial match)
            if (!string.IsNullOrWhiteSpace(areaOfPractice))
            {
                query = query.Where(l => l.AreaOfPractice.ToLower().Contains(areaOfPractice.ToLower()));
            }

            // Filter by minimum years of experience
            if (minYears.HasValue && minYears.Value >= 0)
            {
                query = query.Where(l => l.YearsOfExperience >= minYears.Value);
            }

            // Filter by maximum years of experience
            if (maxYears.HasValue && maxYears.Value >= 0)
            {
                query = query.Where(l => l.YearsOfExperience <= maxYears.Value);
            }

            // Filter by state (exact match, case-insensitive)
            if (!string.IsNullOrWhiteSpace(state))
            {
                query = query.Where(l => l.State.ToLower() == state.ToLower());
            }

            // Filter by city (exact match, case-insensitive)
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(l => l.City.ToLower() == city.ToLower());
            }

            var lawyers = await query.OrderBy(l => l.Name).ToListAsync();
            return Ok(lawyers);
        }

        // GET: api/Lawyers/states
        [HttpGet("states")]
        public async Task<ActionResult<IEnumerable<string>>> GetStates()
        {
            var states = await _context.Lawyers
                .Select(l => l.State)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            return Ok(states);
        }

        // GET: api/Lawyers/cities?state=California
        [HttpGet("cities")]
        public async Task<ActionResult<IEnumerable<string>>> GetCitiesByState([FromQuery] string state)
        {
            if (string.IsNullOrWhiteSpace(state))
            {
                return BadRequest("State parameter is required");
            }

            var cities = await _context.Lawyers
                .Where(l => l.State.ToLower() == state.ToLower())
                .Select(l => l.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return Ok(cities);
        }

        // GET: api/Lawyers/practices
        [HttpGet("practices")]
        public async Task<ActionResult<IEnumerable<string>>> GetPracticeAreas()
        {
            var practices = await _context.Lawyers
                .SelectMany(l => l.AreaOfPractice.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(p => p.Trim())
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            return Ok(practices);
        }

        // GET: api/Lawyers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lawyer>>> GetAllLawyers()
        {
            return await _context.Lawyers.OrderBy(l => l.Name).ToListAsync();
        }
    }
}
