using FarmersPlatform.Data;
using FarmersPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace FarmersPlatform.Controllers
{
    public class BasketController : Controller
    {
        private Basket _basket = Basket.GetInstance();
        private FarmContext _context;

        public BasketController(FarmContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_basket);
        }

        public IActionResult AddProduct(int productId , bool isLoggedIn , int customerId)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                var farmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == product.FarmerId);
                if (product != null && farmer != null)
                {
                    var products = _context.Products.Where(p => p.FarmerId == farmer.FarmerId).ToList();
                    ViewBag.Products = products;
                    ViewBag.CustomerId = customerId;
                    ViewBag.isLoggedIn = isLoggedIn;
                    string productWasAddedMsg = $"{product.Name} was added to cart.";

                    _basket.AddProduct(product);
                    return RedirectToAction("ViewFarmerDetails" , "Customer" , new { farmer.FarmerId , isLoggedIn , customerId, productWasAddedMsg });
                }

                else
                {
                    TempData["ErrorMessage"] = "There has been an error while attempting to add product to basket.";
                    return RedirectToAction("GoToCustomerInterface", new {isLoggedIn, customerId });
                }
            }

            TempData["ErrorMessage"] = "There has been an error while attempting to add product to basket.";
            return RedirectToAction("GoToCustomerInterface", new {isLoggedIn, customerId });
        }

        public IActionResult RemoveProduct(int productId, int farmerId , bool isLoggedIn , int customerId)
        {
            if (ModelState.IsValid)
            {
                var product = _basket.GetProducts().FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    _basket.RemoveProduct(product);
                    return RedirectToAction("ViewFarmerDetails", "Customer", new { farmerId, isLoggedIn, customerId });
                }
                else
                {
                    TempData["ErrorMessage"] = "There has been an error while attempting to add product to basket.";
                    return RedirectToAction("GoToCustomerInterface", new { isLoggedIn, customerId });
                }
            }

            TempData["ErrorMessage"] = "There has been an error while attempting to remove a product from the basket.";
            return RedirectToAction("GoToCustomerInterface", new { isLoggedIn, customerId });
        }
    }

}

