using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.AspNetCore.Identity;

namespace Bank_app_mvc.Models
{
    public class Account
    {
            
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        
        public long AccountNumber { get; set; }
        [Required]
        public string AccountName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 5000;
        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public DateTime OpenedAt { get; set;  } = DateTime.Now;

    }
}
