using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;
using System.Security.Claims;

namespace PCM_357.Pages.Bookings
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly PCMContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(PCMContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public SelectList CourtList { get; set; } = default!;

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public IActionResult OnGet()
        {
            CourtList = new SelectList(_context.Courts.Where(c => c.IsActive), "Id", "Name");
            // Default time
            var now = DateTime.Now;
            var nextFullHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);
            
            Booking = new Booking 
            { 
                StartTime = nextFullHour, 
                EndTime = nextFullHour.AddHours(1) 
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Basic Validation
            if (Booking.StartTime < DateTime.Now)
            {
                ModelState.AddModelError("Booking.StartTime", "Thời gian bắt đầu phải từ hiện tại trở đi.");
            }
            if (Booking.EndTime <= Booking.StartTime)
            {
                ModelState.AddModelError("Booking.EndTime", "Thời gian kết thúc phải lớn hơn bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                CourtList = new SelectList(_context.Courts.Where(c => c.IsActive), "Id", "Name");
                return Page();
            }

            // 2. Overlap Check
            var overlap = await _context.Bookings
                .AnyAsync(b => b.CourtId == Booking.CourtId 
                            && b.Status != BookingStatus.Cancelled
                            && b.StartTime < Booking.EndTime 
                            && b.EndTime > Booking.StartTime);
            
            if (overlap)
            {
                ModelState.AddModelError(string.Empty, "Sân đã có người đặt trong khung giờ này! Vui lòng chọn giờ khác.");
                CourtList = new SelectList(_context.Courts.Where(c => c.IsActive), "Id", "Name");
                return Page();
            }

            // 3. Assign MemberId - Auto-create if needed
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
            
            if (member == null)
            {
                // Auto-create Member record for logged-in user (including Admin)
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng.");
                    CourtList = new SelectList(_context.Courts.Where(c => c.IsActive), "Id", "Name");
                    return Page();
                }

                member = new Member
                {
                    UserId = userId,
                    FullName = user.UserName ?? "User",
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    JoinDate = DateTime.Now,
                    RankLevel = 3.0,
                    IsActive = true,
                    TotalMatches = 0,
                    WinMatches = 0
                };

                _context.Members.Add(member);
                await _context.SaveChangesAsync();
            }

            Booking.MemberId = member.Id;
            Booking.CreatedDate = DateTime.Now;
            Booking.Status = BookingStatus.Confirmed;

            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
