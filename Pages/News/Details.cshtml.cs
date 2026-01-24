using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.News
{
    public class DetailsModel : PageModel
    {
        private readonly PCMContext _context;

        public DetailsModel(PCMContext context)
        {
            _context = context;
        }

        public PCM_357.Entities.News News { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
            if (news == null) return NotFound();
            News = news;
            return Page();
        }
    }
}
