using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM_357.Entities
{
    public enum BookingStatus { Pending, Confirmed, Cancelled }
    public enum ChallengeType { Duel, MiniGame }
    public enum GameMode { None, TeamBattle, RoundRobin }
    public enum ChallengeStatus { Open, Ongoing, Finished }
    public enum MatchFormat { Singles = 1, Doubles = 2 }
    public enum WinningSide { None, Team1, Team2 }
    public enum ParticipantTeam { None, TeamA, TeamB }
    public enum ParticipantStatus { Pending, Confirmed, Withdrawn }

    [Table("357_Bookings")]
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public int CourtId { get; set; }
        [ForeignKey("CourtId")]
        public Court? Court { get; set; }

        public int MemberId { get; set; }
        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    [Table("357_Challenges")]
    public class Challenge
    {
        [Key]
        public int Id { get; set; }
        [Required] [StringLength(200)] public string Title { get; set; } = string.Empty;
        public ChallengeType Type { get; set; }
        public GameMode GameMode { get; set; }
        public ChallengeStatus Status { get; set; } = ChallengeStatus.Open;

        // TeamBattle Config
        public int? Config_TargetWins { get; set; }
        public int CurrentScore_TeamA { get; set; }
        public int CurrentScore_TeamB { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EntryFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrizePool { get; set; }

        public int CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public Member? CreatedBy { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    [Table("357_Participants")]
    public class Participant
    {
        [Key]
        public int Id { get; set; }
        public int ChallengeId { get; set; }
        [ForeignKey("ChallengeId")]
        public Challenge? Challenge { get; set; }

        public int MemberId { get; set; }
        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

        public ParticipantTeam Team { get; set; } = ParticipantTeam.None;
        public bool EntryFeePaid { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal EntryFeeAmount { get; set; }
        public ParticipantStatus Status { get; set; } = ParticipantStatus.Confirmed;
    }

    [Table("357_Matches")]
    public class Match
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsRanked { get; set; }
        public int? ChallengeId { get; set; }
        [ForeignKey("ChallengeId")]
        public Challenge? Challenge { get; set; }

        public MatchFormat MatchFormat { get; set; }

        // TEAM 1
        public int Team1_Player1Id { get; set; }
        [ForeignKey("Team1_Player1Id")] public Member? Team1_Player1 { get; set; }
        public int? Team1_Player2Id { get; set; }
        [ForeignKey("Team1_Player2Id")] public Member? Team1_Player2 { get; set; }

        // TEAM 2
        public int Team2_Player1Id { get; set; }
        [ForeignKey("Team2_Player1Id")] public Member? Team2_Player1 { get; set; }
        public int? Team2_Player2Id { get; set; }
        [ForeignKey("Team2_Player2Id")] public Member? Team2_Player2 { get; set; }

        public WinningSide WinningSide { get; set; } = WinningSide.None;
    }
}
