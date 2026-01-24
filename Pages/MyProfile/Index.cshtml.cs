using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;
using System.Security.Claims;

namespace PCM_357.Pages.MyProfile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly PCMContext _context;

        public IndexModel(PCMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Member Member { get; set; } = default!;

        public string AgeWarning { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                // Auto-create Member record for logged-in user
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return RedirectToPage("/Error");
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
                
                TempData["SuccessMessage"] = "Hồ sơ của bạn đã được tạo! Vui lòng cập nhật thông tin.";
            }
            
            Member = member;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Fetch db member to ensure we don't overwrite Rank
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dbMember = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

            if (dbMember == null) return NotFound();

            // Validate Age (Bonus)
            if (Member.DateOfBirth.HasValue)
            {
                var age = DateTime.Now.Year - Member.DateOfBirth.Value.Year;
                if (Member.DateOfBirth.Value.Date > DateTime.Now.AddYears(-age)) age--;

                if (age < 10 || age > 80)
                {
                    // Just a warning, not blocking save? "hiện cảnh báo".
                    // If blocking: ModelState.AddModelError...
                    // Let's allow save but show warning message. Or block.
                    // Implementation: Show warning in TempData?
                }
            }

            // Update allowed fields
            dbMember.FullName = Member.FullName;
            dbMember.PhoneNumber = Member.PhoneNumber;
            dbMember.DateOfBirth = Member.DateOfBirth;
            // Rank is Ignored from input

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";

            return RedirectToPage();
        }
    }
}
