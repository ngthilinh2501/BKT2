using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Challenges
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<Challenge> Challenges { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Challenges = await _context.Challenges.OrderByDescending(c => c.StartDate).ToListAsync();
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(int id)
        {
            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge != null)
            {
                if (challenge.Status == ChallengeStatus.Open) challenge.Status = ChallengeStatus.Ongoing;
                else if (challenge.Status == ChallengeStatus.Ongoing) challenge.Status = ChallengeStatus.Finished;
                else if (challenge.Status == ChallengeStatus.Finished) challenge.Status = ChallengeStatus.Ongoing;
                
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge != null)
            {
                // Caution: Cascading delete usually handled by DB, but here safeguard is good.
                // Assuming EF Core cascade delete is on or no constraint issue.
                _context.Challenges.Remove(challenge);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
