using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Challenges
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly PCMContext _context;

        public EditModel(PCMContext context)
        {
            _context = context;
        }

        [BindProperty]
        public new Challenge Challenge { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge == null) return NotFound();

            Challenge = challenge;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var challengeToUpdate = await _context.Challenges.FindAsync(Challenge.Id);
            if (challengeToUpdate == null) return NotFound();

            challengeToUpdate.Title = Challenge.Title;
            challengeToUpdate.Type = Challenge.Type;
            challengeToUpdate.GameMode = Challenge.GameMode;
            challengeToUpdate.Config_TargetWins = Challenge.Config_TargetWins;
            challengeToUpdate.EntryFee = Challenge.EntryFee;
            challengeToUpdate.PrizePool = Challenge.PrizePool;
            challengeToUpdate.StartDate = Challenge.StartDate;
            challengeToUpdate.EndDate = Challenge.EndDate;
            challengeToUpdate.Status = Challenge.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Challenges.Any(e => e.Id == Challenge.Id)) return NotFound();
                else throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
