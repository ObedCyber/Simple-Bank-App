using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bank_app_mvc.Models
{
    public class Customer
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        public string Address { get; set; } = string.Empty;
        public string NIN { get; set; } = string.Empty;

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        [ForeignKey("CustomerId")]
        public IdentityUser? User { get; set; }


    }
}
