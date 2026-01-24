using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Bookings
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<Booking> Bookings { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Bookings = await _context.Bookings
                .Include(b => b.Member)
                .Include(b => b.Court)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null && booking.Status != BookingStatus.Cancelled)
            {
                booking.Status = BookingStatus.Cancelled;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
