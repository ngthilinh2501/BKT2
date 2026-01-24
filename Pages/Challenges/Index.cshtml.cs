using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Challenges
{
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<Challenge> ActiveChallenges { get; set; } = default!;
        public IList<Challenge> FinishedChallenges { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var all = await _context.Challenges.Include(c => c.CreatedBy).ToListAsync();
            
            ActiveChallenges = all.Where(c => c.Status != ChallengeStatus.Finished).OrderBy(c => c.Status).ToList();
            FinishedChallenges = all.Where(c => c.Status == ChallengeStatus.Finished).OrderByDescending(c => c.StartDate).ToList();
        }
    }
}
