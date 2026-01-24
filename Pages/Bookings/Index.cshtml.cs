using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;
using System.Security.Claims;

namespace PCM_357.Pages.Bookings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        public IList<Booking> MyBookings { get; set; } = new List<Booking>();
        public IList<Booking> UpcomingBookings { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

            if (member != null)
            {
                var allBookings = await _context.Bookings
                    .Include(b => b.Court)
                    .Where(b => b.MemberId == member.Id)
                    .OrderByDescending(b => b.StartTime)
                    .ToListAsync();
                
                MyBookings = allBookings;

                // Also list upcoming for everyone (optional, or just my upcoming? Requirement says "view calendar" ideally but "view my bookings" is core)
                // Let's stick to My Bookings first.
            }
        }
    }
}
