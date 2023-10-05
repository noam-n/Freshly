using Microsoft.AspNetCore.Identity;

namespace FarmersPlatform.Models
{
    public class FarmerAndBasketViewModel
    {
        public Basket _basket { get; set; }
        public Farmer _farmer { get; set; }

        public FarmerAndBasketViewModel(Farmer farmer , Basket basket)
        {
            _basket = basket;
            _farmer = farmer;
        }
    }
}
