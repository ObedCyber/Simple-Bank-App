using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bank_app_mvc.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Required]
        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Range(1000000000, 9999999999, ErrorMessage = "Account number must be 10 digits.")]
        public long FromAccountNumber { get; set; }

        [Required]
        [Range(1000000000, 9999999999, ErrorMessage = "Account number must be 10 digits.")]
        public long ToAccountNumber { get; set; }

        [ForeignKey("FromAccountNumber")]
        public Account? FromAccount { get; set; }

        [Required]
        [StringLength(50)]
        public string Bank { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string AccountType { get; set; } = string.Empty;
    }


}

