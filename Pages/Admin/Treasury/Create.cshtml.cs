using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Treasury
{
    [Authorize(Roles = "Admin,Treasurer")]
    public class CreateModel : PageModel
    {
        private readonly PCMContext _context;

        public CreateModel(PCMContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> CategoryList { get; set; } = default!;

        [BindProperty]
        public Transaction Transaction { get; set; } = default!;

        public IActionResult OnGet()
        {
            PopulateCategoryList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Transactions == null)
            {
                 PopulateCategoryList();
                return Page();
            }

            _context.Transactions.Add(Transaction);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void PopulateCategoryList()
        {
            var categories = _context.TransactionCategories.OrderBy(c => c.Type).ThenBy(c => c.Name).ToList();
            var incomeGroup = new SelectListGroup { Name = "Khoản Thu" };
            var expenseGroup = new SelectListGroup { Name = "Khoản Chi" };

            var items = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Group = c.Type == TransactionType.Thu ? incomeGroup : expenseGroup
            }).ToList();

            CategoryList = items;
        }
    }
}
