using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmersPlatform.Models
{
    public class Order
    {
        public Order()
        {
            Products = new List<Product>();
        }
            
            

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; } // Primary Key

        public int CustomerId { get; set; } // Foreign Key to Customer
        public int FarmerId { get; set; } // Foreign Key
        public List<Product> Products { get; set; }
        public DateTime OrderDate { get; set; }
        public double FinalPrice { get; set; }
        public bool OrderShipped { get; set; }
        public DateTime ShipDate { get; set; }
        public string? RequestMessage { get; set; }


    }

}
