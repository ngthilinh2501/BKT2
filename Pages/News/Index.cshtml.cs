using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.News
{
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<PCM_357.Entities.News> NewsList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            NewsList = await _context.News
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.CreatedDate)
                .Take(20) // Limit to 20 for now
                .ToListAsync();
        }
    }
}
