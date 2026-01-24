using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using Microsoft.AspNetCore.Mvc;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Courts
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<Court> Courts { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Courts != null)
            {
                Courts = await _context.Courts.ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var court = await _context.Courts.FindAsync(id);

            if (court != null)
            {
                _context.Courts.Remove(court);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
