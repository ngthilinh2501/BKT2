using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly PCMContext _context;

        public MatchesController(PCMContext context)
        {
            _context = context;
        }

        // GET: api/Matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches()
        {
            return await _context.Matches
                .Include(m => m.Team1_Player1)
                .Include(m => m.Team1_Player2)
                .Include(m => m.Team2_Player1)
                .Include(m => m.Team2_Player2)
                .OrderByDescending(m => m.Date)
                .ToListAsync();
        }

        // POST: api/Matches
        [HttpPost]
        public async Task<ActionResult<Match>> PostMatch(Match match)
        {
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatches", new { id = match.Id }, match);
        }
    }
}
