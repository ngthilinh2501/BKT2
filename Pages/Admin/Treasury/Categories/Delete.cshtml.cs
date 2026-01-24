using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Treasury.Categories
{
    [Authorize(Roles = "Admin,Treasurer")]
    public class DeleteModel : PageModel
    {
        private readonly PCMContext _context;

        public DeleteModel(PCMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TransactionCategory Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.TransactionCategories == null)
            {
                return NotFound();
            }

            var category = await _context.TransactionCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.TransactionCategories == null)
            {
                return NotFound();
            }

            var category = await _context.TransactionCategories.FindAsync(id);

            if (category != null)
            {
                Category = category;
                _context.TransactionCategories.Remove(Category);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
