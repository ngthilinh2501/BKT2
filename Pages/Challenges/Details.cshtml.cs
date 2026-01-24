using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Challenges
{
    public class DetailsModel : PageModel
    {
        private readonly PCMContext _context;

        public DetailsModel(PCMContext context)
        {
            _context = context;
        }

        public new Challenge Challenge { get; set; } = default!;
        public IList<Participant> Participants { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Challenge = await _context.Challenges
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Challenge == null) return NotFound();

            Participants = await _context.Participants
                .Include(p => p.Member)
                .Where(p => p.ChallengeId == id)
                .OrderBy(p => p.Team)
                .ToListAsync();

            return Page();
        }
    }
}
