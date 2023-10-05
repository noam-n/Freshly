using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmersPlatform.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; } // Primary Key

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^[0-9]{9,10}$", ErrorMessage = "Phone number must contain up to 10 digits.")]
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; } // Add password field

        public List<Order>? Orders { get; set; } // Navigation property for the list of orders


        // Other properties specific to the Customer model

    }
}
