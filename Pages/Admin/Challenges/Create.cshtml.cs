using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PCM_357.Data;
using PCM_357.Entities;
using System.Security.Claims;

namespace PCM_357.Pages.Admin.Challenges
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly PCMContext _context;

        public CreateModel(PCMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Challenge Challenge { get; set; } = default!;

        public IActionResult OnGet()
        {
            Challenge = new Challenge
            {
                StartDate = DateTime.Now,
                // Defaults
                Type = ChallengeType.MiniGame,
                GameMode = GameMode.TeamBattle,
                Config_TargetWins = 5,
                EntryFee = 50000,
                PrizePool = 500000
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Set Creator
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = _context.Members.FirstOrDefault(m => m.UserId == userId);
            if (member != null)
            {
                Challenge.CreatedById = member.Id;
            }
            else
            {
                // Fallback for Admin
                 Challenge.CreatedById = _context.Members.First().Id;
            }

            Challenge.Status = ChallengeStatus.Open;
            Challenge.CurrentScore_TeamA = 0;
            Challenge.CurrentScore_TeamB = 0;

            _context.Challenges.Add(Challenge);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
