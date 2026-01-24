using Microsoft.AspNetCore.Identity;
using PCM_357.Entities;

namespace PCM_357.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(PCMContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // 1. Roles
            string[] roles = { "Admin", "Member", "Referee", "Treasurer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Admin User
            var adminEmail = "admin@pcm.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 2.1. Treasurer User
            var treasurerEmail = "treasurer@pcm.com";
            var treasurerUser = await userManager.FindByEmailAsync(treasurerEmail);
            if (treasurerUser == null)
            {
                treasurerUser = new IdentityUser { UserName = treasurerEmail, Email = treasurerEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(treasurerUser, "Treasurer@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(treasurerUser, "Treasurer");
                }
            }

            // 2.2. Referee User
            var refereeEmail = "referee@pcm.com";
            var refereeUser = await userManager.FindByEmailAsync(refereeEmail);
            if (refereeUser == null)
            {
                refereeUser = new IdentityUser { UserName = refereeEmail, Email = refereeEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(refereeUser, "Referee@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(refereeUser, "Referee");
                }
            }

            // 3. Courts
            if (!context.Courts.Any())
            {
                context.Courts.AddRange(
                    new Court { Name = "Sân 1", IsActive = true, Description = "Sân tiêu chuẩn" },
                    new Court { Name = "Sân 2", IsActive = true, Description = "Sân thi đấu" }
                );
                await context.SaveChangesAsync();
            }

            // 4. Transaction Categories
            if (!context.TransactionCategories.Any())
            {
                context.TransactionCategories.AddRange(
                    new TransactionCategory { Name = "Tiền sân", Type = TransactionType.Thu },
                    new TransactionCategory { Name = "Quỹ tháng", Type = TransactionType.Thu },
                    new TransactionCategory { Name = "Nước", Type = TransactionType.Chi },
                    new TransactionCategory { Name = "Phạt", Type = TransactionType.Chi }
                );
                await context.SaveChangesAsync();
            }

            // 5. Transactions (Make Fund Positive)
            if (!context.Transactions.Any())
            {
                // Need Ids of categories
                var catQuy = context.TransactionCategories.First(c => c.Name == "Quỹ tháng").Id;
                var catNuoc = context.TransactionCategories.First(c => c.Name == "Nước").Id;

                context.Transactions.AddRange(
                    new Transaction { Amount = 1000000, CategoryId = catQuy, Description = "Thu quỹ tháng 1", Date = DateTime.Now.AddDays(-5) },
                    new Transaction { Amount = 200000, CategoryId = catNuoc, Description = "Mua nước suối", Date = DateTime.Now.AddDays(-2) }
                ); // Balance = 800k
                await context.SaveChangesAsync();
            }

            // 6. Members
            if (!context.Members.Any())
            {
                for (int i = 1; i <= 6; i++)
                {
                    var email = $"member{i}@pcm.com";
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                        await userManager.CreateAsync(user, "Member@123");
                        await userManager.AddToRoleAsync(user, "Member");
                    }

                    context.Members.Add(new Member
                    {
                        FullName = $"Member {i}",
                        Email = email,
                        UserId = user.Id,
                        RankLevel = 3.0 + (i * 0.1), // 3.1, 3.2...
                        JoinDate = DateTime.Now.AddMonths(-1)
                    });
                }
                await context.SaveChangesAsync();
            }

            // 7. Challenges & Matches (Ongoing)
            if (!context.Challenges.Any())
            {
                var admin = context.Members.First(); // Assume CreatedBy Admin or first member
                context.Challenges.Add(new Challenge
                {
                    Title = "Mini-Game Khai Xuân",
                    Type = ChallengeType.MiniGame,
                    GameMode = GameMode.TeamBattle,
                    Status = ChallengeStatus.Ongoing,
                    Config_TargetWins = 5,
                    EntryFee = 50000,
                    PrizePool = 500000,
                    CreatedById = admin.Id,
                    StartDate = DateTime.Now.AddDays(-1),
                    CurrentScore_TeamA = 1,
                    CurrentScore_TeamB = 1
                });
                await context.SaveChangesAsync();

                var challenge = context.Challenges.First();
                var members = context.Members.Take(4).ToList(); // Get 4 members

                // Participants
                if (members.Count >= 4)
                {
                    context.Participants.AddRange(
                        new Participant { ChallengeId = challenge.Id, MemberId = members[0].Id, Team = ParticipantTeam.TeamA, EntryFeePaid = true },
                        new Participant { ChallengeId = challenge.Id, MemberId = members[1].Id, Team = ParticipantTeam.TeamA, EntryFeePaid = true },
                        new Participant { ChallengeId = challenge.Id, MemberId = members[2].Id, Team = ParticipantTeam.TeamB, EntryFeePaid = true },
                        new Participant { ChallengeId = challenge.Id, MemberId = members[3].Id, Team = ParticipantTeam.TeamB, EntryFeePaid = true }
                    );
                    await context.SaveChangesAsync();

                    // Matches
                    context.Matches.AddRange(
                        // Match 1: Team A vs Team B (Singles)
                        new Match
                        {
                            Date = DateTime.Now.AddHours(-2),
                            IsRanked = true,
                            ChallengeId = challenge.Id,
                            MatchFormat = MatchFormat.Singles,
                            Team1_Player1Id = members[0].Id, // A
                            Team2_Player1Id = members[2].Id, // B
                            WinningSide = WinningSide.Team1 // A wins
                        },
                        // Match 2: Team A vs Team B (Doubles)
                        new Match
                        {
                            Date = DateTime.Now.AddHours(-1),
                            IsRanked = true,
                            ChallengeId = challenge.Id,
                            MatchFormat = MatchFormat.Doubles,
                            Team1_Player1Id = members[0].Id, Team1_Player2Id = members[1].Id, // A
                            Team2_Player1Id = members[2].Id, Team2_Player2Id = members[3].Id, // B
                            WinningSide = WinningSide.Team2 // B wins
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
