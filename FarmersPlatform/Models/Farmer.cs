using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmersPlatform.Models
{
    public class Farmer
    {

        public Farmer()
        {
            if (Products == null)
            {
                Products = new List<Product>();
            }
            Orders = new List<Order>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FarmerId { get; set; } // Primary Key

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^[0-9]{9,10}$", ErrorMessage = "Phone number must contain up to 10 digits.")]
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Image { get; set; } // Image URL or file path

        public string? AboutMyFarm { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; } // Add password field

        public string? PlatformJoinDate { get; set; }
        public List<Product> Products { get; set; } // Navigation property
        public List<Order> Orders { get; set; } // Navigation property



    }


}
