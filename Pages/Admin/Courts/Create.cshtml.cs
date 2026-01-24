using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Courts
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly PCMContext _context;

        public CreateModel(PCMContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Court Court { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Courts == null || Court == null)
            {
                return Page();
            }

            _context.Courts.Add(Court);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
