using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using Microsoft.AspNetCore.Mvc;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Members
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<Member> Members { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Members != null)
            {
                Members = await _context.Members
                    .OrderByDescending(m => m.RankLevel)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member != null)
            {
                try
                {
                    _context.Members.Remove(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Handle foreign key constraint violations
                     TempData["Error"] = "Không thể xóa hội viên này do có dữ liệu liên quan (lịch sử đấu, đặt sân).";
                     return RedirectToPage();
                }
            }

            return RedirectToPage();
        }
    }
}
