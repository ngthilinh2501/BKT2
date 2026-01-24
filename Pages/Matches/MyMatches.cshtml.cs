using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;
using System.Security.Claims;

namespace PCM_357.Pages.Matches
{
    [Authorize]
    public class MyMatchesModel : PageModel
    {
        private readonly PCMContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MyMatchesModel(PCMContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Match> Matches { get; set; } = new List<Match>();
        public int CurrentMemberId { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                // Auto-create Member record for logged-in user
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
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
            }

            if (member != null)
            {
                CurrentMemberId = member.Id;
                
                // Get all matches for debugging
                var allMatches = await _context.Matches.ToListAsync();
                
                Matches = await _context.Matches
                    .Include(m => m.Team1_Player1)
                    .Include(m => m.Team1_Player2)
                    .Include(m => m.Team2_Player1)
                    .Include(m => m.Team2_Player2)
                    .Include(m => m.Challenge)
                    .Where(m => m.Team1_Player1Id == member.Id || m.Team1_Player2Id == member.Id 
                             || m.Team2_Player1Id == member.Id || m.Team2_Player2Id == member.Id)
                    .OrderByDescending(m => m.Date)
                    .ToListAsync();
                
                // Debug: Add info to TempData
                if (Matches.Count == 0 && allMatches.Count > 0)
                {
                    TempData["DebugInfo"] = $"Tổng số trận đấu trong database: {allMatches.Count}. Member ID của bạn: {member.Id}. Không tìm thấy trận đấu nào có Member ID này.";
                }
            }
        }
    }
}
