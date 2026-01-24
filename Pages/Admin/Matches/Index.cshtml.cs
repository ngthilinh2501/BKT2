using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Matches
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<Match> Matches { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Matches = await _context.Matches
                .Include(m => m.Team1_Player1)
                .Include(m => m.Team2_Player1)
                .OrderByDescending(m => m.Date)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match != null)
            {
                // Reverting rank changes is complex, for now just allow delete.
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
