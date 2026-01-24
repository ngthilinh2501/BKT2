using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCM_357.Data;
using PCM_357.Entities;

namespace PCM_357.Pages.Matches
{
    [Authorize(Roles = "Admin,Referee")]
    public class CreateModel : PageModel
    {
        private readonly PCMContext _context;

        public CreateModel(PCMContext context)
        {
            _context = context;
        }

        public SelectList MemberList { get; set; } = default!;
        public SelectList ChallengeList { get; set; } = default!;

        [BindProperty]
        public Match Match { get; set; } = default!;

        public IActionResult OnGet()
        {
            var members = _context.Members.Where(m => m.IsActive).Select(m => new { m.Id, FullName = m.FullName + " (" + m.RankLevel.ToString("0.0") + ")" }).ToList();
            
            // Check if we have enough members
            if (members.Count < 2)
            {
                TempData["ErrorMessage"] = "Cần ít nhất 2 hội viên để tạo trận đấu. Hiện tại chỉ có " + members.Count + " hội viên. Vui lòng đảm bảo các hội viên đã truy cập 'Hồ sơ của tôi' để tạo profile.";
                return RedirectToPage("/Index");
            }
            
            MemberList = new SelectList(members, "Id", "FullName");
            ChallengeList = new SelectList(_context.Challenges.Where(c => c.Status != ChallengeStatus.Finished), "Id", "Title");
            
            Match = new Match { Date = DateTime.Now, MatchFormat = MatchFormat.Singles, IsRanked = true };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validation: Check for duplicate players
            if (Match.Team1_Player1Id == Match.Team2_Player1Id)
            {
                ModelState.AddModelError("", "Người chơi không được trùng nhau.");
            }

            // Additional validation for Doubles
            if (Match.MatchFormat == MatchFormat.Doubles)
            {
                if (!Match.Team1_Player2Id.HasValue || !Match.Team2_Player2Id.HasValue)
                {
                    ModelState.AddModelError("", "Trận đấu đôi phải có đủ 4 người chơi.");
                }
                else
                {
                    // Check all 4 players are unique
                    var playerIds = new List<int> 
                    { 
                        Match.Team1_Player1Id, 
                        Match.Team1_Player2Id.Value, 
                        Match.Team2_Player1Id, 
                        Match.Team2_Player2Id.Value 
                    };
                    
                    if (playerIds.Distinct().Count() != 4)
                    {
                        ModelState.AddModelError("", "Tất cả người chơi phải khác nhau.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                // Reload lists
                var members = _context.Members.Where(m => m.IsActive).Select(m => new { m.Id, FullName = m.FullName + " (" + m.RankLevel.ToString("0.0") + ")" }).ToList();
                MemberList = new SelectList(members, "Id", "FullName");
                ChallengeList = new SelectList(_context.Challenges.Where(c => c.Status != ChallengeStatus.Finished), "Id", "Title");
                return Page();
            }

            // 1. Save Match
            _context.Matches.Add(Match);

            // 2. Update Ranks (If Ranked)
            if (Match.IsRanked && Match.WinningSide != WinningSide.None)
            {
                await UpdateRanks(Match);
            }

            // 3. Update Challenge (If Linked)
            if (Match.ChallengeId.HasValue)
            {
                await UpdateChallengeState(Match);
            }

            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Đã ghi nhận kết quả trận đấu thành công!";
            return RedirectToPage("/Matches/MyMatches");
        }

        private async Task UpdateRanks(Match match)
        {
            var p1 = await _context.Members.FindAsync(match.Team1_Player1Id);
            var p2 = await _context.Members.FindAsync(match.Team2_Player1Id);
            
            // Simple Logic: Winner +0.1, Loser -0.1
            // Refine: Only update if not null
            if (p1 == null || p2 == null) return;

            if (match.WinningSide == WinningSide.Team1)
            {
                p1.RankLevel += 0.1;
                p1.WinMatches++;
                p2.RankLevel -= 0.1;
            }
            else if (match.WinningSide == WinningSide.Team2)
            {
                p1.RankLevel -= 0.1;
                p2.RankLevel += 0.1;
                p2.WinMatches++;
            }
            p1.TotalMatches++;
            p2.TotalMatches++;
            
            // Doubles logic similar... (省略 for simplicity in this snippet, can expand if doubles selected)
            if (match.MatchFormat == MatchFormat.Doubles && match.Team1_Player2Id.HasValue && match.Team2_Player2Id.HasValue)
            {
                 var p1b = await _context.Members.FindAsync(match.Team1_Player2Id);
                 var p2b = await _context.Members.FindAsync(match.Team2_Player2Id);
                 if (p1b != null && p2b != null)
                 {
                    if (match.WinningSide == WinningSide.Team1)
                    {
                        p1b.RankLevel += 0.1; p1b.WinMatches++;
                        p2b.RankLevel -= 0.1;
                    }
                    else
                    {
                        p1b.RankLevel -= 0.1;
                        p2b.RankLevel += 0.1; p2b.WinMatches++;
                    }
                    p1b.TotalMatches++;
                    p2b.TotalMatches++;
                 }
            }
        }

        private async Task UpdateChallengeState(Match match)
        {
            var challenge = await _context.Challenges.FindAsync(match.ChallengeId);
            if (challenge == null) return;

            if (challenge.GameMode == GameMode.TeamBattle)
            {
                if (match.WinningSide == WinningSide.Team1) challenge.CurrentScore_TeamA++; // Assume Team1 is Team A mapping logic? 
                // Wait, need to know which Team matches Team1. Complex.
                // Simplified: Assume Team 1 is ALWAYS Team A players, Team 2 is Team B.
                // Or check Participant table.
                // For logic safety: If Winner is in Team A -> Score A++.
                
                var winnerId = match.WinningSide == WinningSide.Team1 ? match.Team1_Player1Id : match.Team2_Player1Id;
                var participant = await _context.Participants.FirstOrDefaultAsync(p => p.ChallengeId == challenge.Id && p.MemberId == winnerId);
                
                if (participant != null)
                {
                    if (participant.Team == ParticipantTeam.TeamA) challenge.CurrentScore_TeamA++;
                    else if (participant.Team == ParticipantTeam.TeamB) challenge.CurrentScore_TeamB++;
                }

                if (challenge.Config_TargetWins.HasValue)
                {
                    if (challenge.CurrentScore_TeamA >= challenge.Config_TargetWins || challenge.CurrentScore_TeamB >= challenge.Config_TargetWins)
                    {
                        challenge.Status = ChallengeStatus.Finished;
                    }
                }
            }
            else if (challenge.GameMode == GameMode.RoundRobin)
            {
                // RoundRobin Logic:
                // Points are calculated dynamically. Global Rank update covers player status.
            }
        }
    }
}
