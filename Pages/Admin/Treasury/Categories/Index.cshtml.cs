using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Treasury.Categories
{
    [Authorize(Roles = "Admin,Treasurer")]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<TransactionCategory> Categories { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Categories = await _context.TransactionCategories.ToListAsync();
        }
    }
}
