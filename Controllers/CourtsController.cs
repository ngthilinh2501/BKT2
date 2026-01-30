using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtsController : ControllerBase
    {
        private readonly PCMContext _context;

        public CourtsController(PCMContext context)
        {
            _context = context;
        }

        // GET: api/Courts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Court>>> GetCourts()
        {
            return await _context.Courts.ToListAsync();
        }

        // GET: api/Courts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Court>> GetCourt(int id)
        {
            var court = await _context.Courts.FindAsync(id);

            if (court == null)
            {
                return NotFound();
            }

            return court;
        }
    }
}
