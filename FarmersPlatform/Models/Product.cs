using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmersPlatform.Models
{
    public class Product

    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; } // Primary Key

        public string? Name { get; set; }
        public double Price { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; } // Image URL or file path

        public int Quantity { get; set; }
        public int FarmerId { get; set; } // Foreign key
    }
}





