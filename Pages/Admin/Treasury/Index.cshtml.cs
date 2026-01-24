using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Treasury
{
    [Authorize(Roles = "Admin,Treasurer")]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<Transaction> Transactions { get; set; } = default!;
        public decimal TotalFund { get; set; }
        public decimal TotalThu { get; set; }
        public decimal TotalChi { get; set; }

        public async Task OnGetAsync()
        {
            if (_context.Transactions != null)
            {
                Transactions = await _context.Transactions
                    .Include(t => t.Category)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();
                
                // Calculate Stats
                TotalThu = Transactions.Where(t => t.Category.Type == TransactionType.Thu).Sum(t => t.Amount);
                TotalChi = Transactions.Where(t => t.Category.Type == TransactionType.Chi).Sum(t => t.Amount);
                TotalFund = TotalThu - TotalChi;
            }
        }
    }
}
