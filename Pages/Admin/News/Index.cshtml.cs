using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.News
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<PCM_357.Entities.News> News { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.News != null)
            {
                News = await _context.News.OrderByDescending(n => n.CreatedDate).ToListAsync();
            }
        }
    }
}
