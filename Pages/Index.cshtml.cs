using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<PCM_357.Entities.News> PinnedNews { get; set; } = new List<PCM_357.Entities.News>();
        public IList<Member> TopPlayers { get; set; } = new List<Member>();
        public decimal TotalFund { get; set; }
        public int ActiveChallengeCount { get; set; }
        public bool IsFundNegative => TotalFund < 0;

        public async Task OnGetAsync()
        {
            // 1. Get Latest/Pinned News
            PinnedNews = await _context.News
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.CreatedDate)
                .Take(3)
                .ToListAsync();

            // 2. Get Top 5 Rankings
            TopPlayers = await _context.Members
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.RankLevel)
                .Take(5)
                .ToListAsync();

            // 3. Get Active Challenges
            ActiveChallengeCount = await _context.Challenges
                .Where(c => c.Status != ChallengeStatus.Finished)
                .CountAsync();

            // 4. Calc Fund
            var thu = await _context.Transactions
                .Where(t => t.Category.Type == TransactionType.Thu)
                .SumAsync(t => t.Amount);
            var chi = await _context.Transactions
                .Where(t => t.Category.Type == TransactionType.Chi)
                .SumAsync(t => t.Amount);
            TotalFund = thu - chi;
        }
    }
}
