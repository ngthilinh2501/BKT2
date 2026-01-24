using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PCM_357.Entities
{
    [Table("357_Members")]
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public double RankLevel { get; set; } = 3.0; // Default DUPR start

        public bool IsActive { get; set; } = true;

        // Identity Link
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Stats
        public int TotalMatches { get; set; }
        public int WinMatches { get; set; }
    }

    [Table("357_News")]
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsPinned { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    [Table("357_Courts")]
    public class Court
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty; // Sân 1, Sân 2

        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
    }
}
