using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Treasury
{
    [Authorize(Roles = "Admin,Treasurer")]
    public class EditModel : PageModel
    {
        private readonly PCMContext _context;

        public EditModel(PCMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Transaction Transaction { get; set; } = default!;

        public IEnumerable<SelectListItem> CategoryList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }
            Transaction = transaction;
            PopulateCategoryList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateCategoryList();
                return Page();
            }

            _context.Attach(Transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(Transaction.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TransactionExists(int id)
        {
          return (_context.Transactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private void PopulateCategoryList()
        {
            var categories = _context.TransactionCategories.OrderBy(c => c.Type).ThenBy(c => c.Name).ToList();
            var incomeGroup = new SelectListGroup { Name = "Khoản Thu" };
            var expenseGroup = new SelectListGroup { Name = "Khoản Chi" };

            CategoryList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Group = c.Type == TransactionType.Thu ? incomeGroup : expenseGroup,
                Selected = c.Id == Transaction.CategoryId
            }).ToList();
        }
    }
}
