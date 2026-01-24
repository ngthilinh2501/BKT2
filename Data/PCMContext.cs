using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCM_357.Entities;

namespace PCM_357.Data
{
    public class PCMContext : IdentityDbContext
    {
        public PCMContext(DbContextOptions<PCMContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Fluent API configurations if needed
            // Example: Prevent cascade delete loops or specific constraints
            
            // Match relations usually have multiple FKs to Members, so we need to restrict delete behavior
            builder.Entity<Match>().HasOne(m => m.Team1_Player1).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Match>().HasOne(m => m.Team1_Player2).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Match>().HasOne(m => m.Team2_Player1).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Match>().HasOne(m => m.Team2_Player2).WithMany().OnDelete(DeleteBehavior.NoAction);

            // Fix Cascade Cycles for SQL Server
            builder.Entity<Participant>()
                .HasOne(p => p.Member)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // Member delete won't auto-delete participant history (or handle manually)

             builder.Entity<Challenge>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Booking>()
                .HasOne(b => b.Member)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
