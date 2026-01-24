using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.News
{
    [Authorize(Roles = "Admin,NewsEditor")]
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
        public PCM_357.Entities.News News { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.News == null || News == null)
            {
                return Page();
            }

            News.CreatedDate = DateTime.Now;
            _context.News.Add(News);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
