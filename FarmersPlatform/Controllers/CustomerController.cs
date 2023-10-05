using FarmersPlatform.Data;
using FarmersPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmersPlatform.Controllers
{
    public class CustomerController : Controller
    {

        private FarmContext _context;
        bool isLoggedIn = false;

        public CustomerController(FarmContext context)
        {
            _context = context;
        }

        public IActionResult Login(string email, string password)
        {

            var customer = _context.Customers.FirstOrDefault(c => c.Password == password && c.Email == email);

            if (customer != null)
            {
                ViewBag.isLoggedIn = true;
                ViewBag.CustomerFirstName = customer.FirstName;
                return RedirectToAction("GoToCustomerInterface", new { isLoggedIn = true, customerId = customer.CustomerId });
            }
            else
            {
                TempData["ErrorMessage"] = "Email or Password incorrect - please try again.";
                return View("CustomerLoginView");
            }
        }

        public IActionResult Logout(int customerId)
        {
            isLoggedIn = false;
            return RedirectToAction("GoToCustomerInterface", new { isLoggedIn = false, customerId });
        }

        public IActionResult Signup(Customer customer)
        {

            if (ModelState.IsValid)
            {
                if (_context.Customers.Any(c => c.Email == customer.Email))
                {
                    TempData["ErrorMessage"] = "Email already exists. If you already have an account - please login , otherwise please use a different email.";
                    return View("CustomerSignupView");
                }

                _context.Customers.Add(customer);
                _context.SaveChanges();
                isLoggedIn = true;
                return RedirectToAction("GoToCustomerInterface", new { isLoggedIn, customer.CustomerId });
            }

            // If the form inputs are not valid, return the signup view with validation errors
            TempData["ErrorMessage"] = "There has been an error while attempting to sign-up, please try again.";
            return View("CustomerSignupView");
        }

        public IActionResult GoToCustomerInterface(bool isLoggedIn, int customerId)
        {

            if (TempData["OrderConfirmationMessage"] != null)
            {
                ViewBag.OrderConfirmationMessage = TempData["OrderConfirmationMessage"].ToString();
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer != null)
            {
                ViewBag.IsLoggedIn = isLoggedIn;
                ViewBag.CustomerFirstName = customer.FirstName;
                ViewBag.Farmers = _context.Farmers.ToList();

                return View("CustomerInterface", customer);
            }
            else
            {
                // Handle the case when the customer is not found
                ViewBag.IsLoggedIn = false;
                ViewBag.Farmers = _context.Farmers.ToList();
                ViewBag.CustomerId = 100000;
                return View("CustomerInterface");
            }
        }

        public IActionResult PersonalDetails(int customerId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer != null)
            {
                return View("PersonalDetailsCustomer", customer);
            }
            else
            {
                return RedirectToAction("GoToCustomerInterface", new { isLoggedIn = false, customerId });
            }
        }

        [HttpPost]
        public IActionResult UpdatePersonalDetails(Customer updatedCustomer)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing customer from the database
                if (updatedCustomer != null)
                {

                    var existingCustomer = _context.Customers.FirstOrDefault(c => c.CustomerId == updatedCustomer.CustomerId);
                    if (existingCustomer != null)
                    {
                        // Update the properties of the existing customer with the new values
                        existingCustomer.FirstName = updatedCustomer.FirstName;
                        existingCustomer.LastName = updatedCustomer.LastName;
                        existingCustomer.Email = updatedCustomer.Email;
                        existingCustomer.PhoneNumber = updatedCustomer.PhoneNumber;
                        existingCustomer.Address = updatedCustomer.Address;

                        _context.SaveChanges();
                        TempData["UpdateSuccess"] = true;

                        // Redirect to the personal details page or perform any other action
                        return RedirectToAction("PersonalDetails", new { customerId = updatedCustomer.CustomerId });
                    }
                    else
                    {
                        // Handle the case when the customer is not found
                        return View("PersonalDetailsCustomer", updatedCustomer);
                    }
                }
                return RedirectToAction("GoToCustomerInterface", new { isLoggedIn = false });
            }
            else
            {
                // If the model is not valid, return the view with validation errors
                return RedirectToAction("GoToCustomerInterface", new { isLoggedIn = false });
            }
        }

        public ActionResult ViewFarmerDetails(int farmerId, bool isLoggedIn, int? customerId, string? productWasAddedMsg)
        {
            var farmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == farmerId);
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (farmer != null)
            {
                var products = _context.Products.Where(p => p.FarmerId == farmer.FarmerId).ToList();
                var distinctProducts = products.GroupBy(p => p.Name).Select(g => g.First()).ToList();

                ViewBag.Products = distinctProducts; // make sure only 1 instance of each fruit / veg

                ViewBag.CustomerId = customerId;
                if (customer is null)
                {
                    ViewBag.CustomerFirstName = "";
                }
                else
                {
                    ViewBag.CustomerFirstName = customer.FirstName;
                }

                if(productWasAddedMsg != null)
                    ViewBag.ProductAddedToBasketSuccuess = productWasAddedMsg;

                ViewBag.isLoggedIn = isLoggedIn;
                var _basket = Basket.GetInstance();
                FarmerAndBasketViewModel FarmerAndBasketModel = new FarmerAndBasketViewModel(farmer, _basket);

                return View("FarmerDetailsFromCust", FarmerAndBasketModel);
            }
            else if (isLoggedIn && customer != null)
            {
                TempData["ErrorNavigatingToFarmerDetails"] = "There has been an error - Please try again.";
                return RedirectToAction("GoToCustomerInterface", new { isLoggedIn = true, customer });
            }
            else if (isLoggedIn)
            {
                TempData["ErrorNavigatingToFarmerDetails"] = "There has been an error - Please try again.";
                return RedirectToAction("GoToCustomerInterface", new { isLoggedIn = true });
            }
            else
            {
                TempData["ErrorNavigatingToFarmerDetails"] = "There has been an error - Please try again.";
                return RedirectToAction("GoToCustomerInterface", new { isLoggedIn = false });
            }
        }

        public IActionResult CustomerOrders(int customerId ,int? farmerId)
        {
            
                var farmer = _context.Farmers
                                .Include(f => f.Products).
                                 Include(f => f.Orders)               //Eager loading of Products navigation property
                                .FirstOrDefault(f => f.FarmerId == farmerId);

            var customer = _context.Customers.Include(c => c.Orders)
                    .ThenInclude(o => o.Products)  // Eager loading of Products for each Order
                    .FirstOrDefault(c => c.CustomerId == customerId);

            var ordersWithOrderIdBiggerThan30 = customer.Orders.Where(o => o.OrderId >= 30).ToList();

            if(farmer != null)
            {
                ViewBag.Farmer = farmer;
                ViewBag.FarmerFullName = farmer.FirstName + " " + farmer.LastName;
                ViewBag.FarmerEmail = farmer.Email;
                ViewBag.FarmerId = farmer.FarmerId;
            }


            ViewBag.Orders = ordersWithOrderIdBiggerThan30;
            ViewBag.OrdersLength = ordersWithOrderIdBiggerThan30.Count();
            return View("CustomerOrders", customer);
        }

        public IActionResult GetFarmerDetails(int customerId, int? farmerId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            var farmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == farmerId);
            if (customer == null)
            {
                return RedirectToAction("GoToCustomerInterface", new { isLoggedIn = false });
            }
            return RedirectToAction("CustomerOrders", new { customerId, farmerId });
        }

        public IActionResult GoToVegetables(bool isLoggedIn, int customerId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            var farmers = _context.Farmers.Include(f => f.Products);

            // Retrieve the list of farmers from your data context and filter their products

            //var vegetableProducts = farmers
            //    .SelectMany(farmer => farmer.Products.Where(p => p.Category == "Vegetable"))
            //    .ToList();

            var vegetableProducts = farmers
            .SelectMany(farmer => farmer.Products.Where(p => p.Category == "Vegetable"))
            .GroupBy(p => p.Name)
            .Select(group => group.First())
            .ToList();

            ViewBag.Basket = Basket.GetInstance();
            ViewBag.isLoggedIn = isLoggedIn;
            ViewBag.AllVegetables = vegetableProducts;

            return View("AllVegetables", customer);
        }

    }
}
