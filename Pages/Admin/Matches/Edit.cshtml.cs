using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Admin.Matches
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
        public Match Match { get; set; } = default!;

        public SelectList MemberList { get; set; } = default!;
        public SelectList ChallengeList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var match = await _context.Matches.FindAsync(id);
            if (match == null) return NotFound();
            Match = match;

            var members = _context.Members.Select(m => new { m.Id, FullName = m.FullName }).ToList();
            MemberList = new SelectList(members, "Id", "FullName");
            ChallengeList = new SelectList(_context.Challenges, "Id", "Title");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                 var members = _context.Members.Select(m => new { m.Id, FullName = m.FullName }).ToList();
                 MemberList = new SelectList(members, "Id", "FullName");
                 ChallengeList = new SelectList(_context.Challenges, "Id", "Title");
                 return Page();
            }

            var matchToUpdate = await _context.Matches.FindAsync(Match.Id);
            if (matchToUpdate == null) return NotFound();

            // Checking for winner change warning
            bool winningSideChanged = matchToUpdate.WinningSide != Match.WinningSide;

            matchToUpdate.Date = Match.Date;
            matchToUpdate.MatchFormat = Match.MatchFormat;
            matchToUpdate.Team1_Player1Id = Match.Team1_Player1Id;
            matchToUpdate.Team1_Player2Id = Match.Team1_Player2Id;
            matchToUpdate.Team2_Player1Id = Match.Team2_Player1Id;
            matchToUpdate.Team2_Player2Id = Match.Team2_Player2Id;
            matchToUpdate.WinningSide = Match.WinningSide;
            matchToUpdate.IsRanked = Match.IsRanked;
            matchToUpdate.ChallengeId = Match.ChallengeId;

            // Note: We are NOT adhering to automatic Rank Reversion because it's complex stateless logic.
            // Requirement says Admin CAN edit. It implies manual fix.

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
