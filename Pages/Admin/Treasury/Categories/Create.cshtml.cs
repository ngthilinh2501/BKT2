using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Treasury.Categories
{
    [Authorize(Roles = "Admin,Treasurer")]
    public class CreateModel : PageModel
    {
        private readonly PCMContext _context;

        public CreateModel(PCMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TransactionCategory NewCategory { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TransactionCategories.Add(NewCategory);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }
    }
}
