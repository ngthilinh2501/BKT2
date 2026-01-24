using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM_357.Entities
{
    public enum TransactionType
    {
        Thu = 1,
        Chi = 2
    }

    [Table("357_TransactionCategories")]
    public class TransactionCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Tiền sân, Nước...

        public TransactionType Type { get; set; }
    }

    [Table("357_Transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public TransactionCategory? Category { get; set; }

        public int? CreatedById { get; set; }
        // [ForeignKey("CreatedById")]
        // public Member? CreatedBy { get; set; } 
    }
}
