using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.News
{
    [Authorize(Roles = "Admin,NewsEditor")]
    public class EditModel : PageModel
    {
        private readonly PCMContext _context;

        public EditModel(PCMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PCM_357.Entities.News News { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.News == null) return NotFound();

            var news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
            if (news == null) return NotFound();
            News = news;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Attach(News).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.News.Any(e => e.Id == News.Id)) 
                    return NotFound();
                else 
                    throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
