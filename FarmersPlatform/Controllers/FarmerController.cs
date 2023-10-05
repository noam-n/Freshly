using FarmersPlatform.Data;
using FarmersPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp;

namespace FarmersPlatform.Controllers
{
    public class FarmerController : Controller
    {

        private FarmContext _context;
        bool isLoggedIn = false;

        public FarmerController(FarmContext context)
        {
            _context = context;
        }

        public IActionResult Login(string email, string password)
        {
            var farmer = _context.Farmers.FirstOrDefault(f => f.Password == password && f.Email == email);
            if (farmer != null)
            {
                ViewBag.isLoggedIn = true;
                ViewBag.FarmerFirstName = farmer.FirstName;
                return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = true, farmerId = farmer.FarmerId });
            }
            else
            {
                TempData["ErrorMessage"] = "Email or Password incorrect - please try again.";
                return View("FarmerLoginView");
            }
        }

        public IActionResult Signup(Farmer farmer, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (_context.Farmers.Any(f => f.Email == farmer.Email))
                {
                    TempData["ErrorMessage"] = "Email already exists. If you already have an account - please login , otherwise please use a different email.";
                    return View("FarmerSignupView");
                }
                if (image != null && image.Length > 0)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }
                    // Set the image path in the farmer object
                    farmer.Image = fileName;
                }

                farmer.PlatformJoinDate = DateTime.Now.ToShortDateString();
                _context.Farmers.Add(farmer);
                _context.SaveChanges();
                //return View("FarmerInterface");
                return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = true, farmer.FarmerId });
            }

            TempData["ErrorMessage"] = "There has been an error while attempting to sign-up.";
            return View("FarmerSignupView");
        }

        public IActionResult Logout(int farmerId)
        {
            isLoggedIn = false;
            return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = false, farmerId });
        }

        public IActionResult GoToFarmerInterface(bool isLoggedIn, int farmerId)
        {
            if (isLoggedIn == false)
            {
                return View("FarmerLoginView");
            }

            var farmer = _context.Farmers.Include(f => f.Products) // Eager loading of Products navigation property
            .FirstOrDefault(f => f.FarmerId == farmerId);

            if (farmer != null)
            {
                ViewBag.IsLoggedIn = isLoggedIn;
                ViewBag.FarmerFirstName = farmer.FirstName;
                if (farmer.Products == null)
                {
                    farmer.Products = new List<Product>();
                }
                ViewBag.Products = farmer.Products.GroupBy(p => p.Name)
                .Select(group => group.First())
                .ToList();

                return View("FarmerInterface", farmer);
            }
            else
            {
                // Handle the case when the customer is not found
                return View("FarmerLoginView");
            }
        }

        [HttpPost]
        public IActionResult UpdatePersonalDetails(Farmer updatedFarmer, IFormFile? image = null)
        {
            if (ModelState.IsValid)
            {
                if (updatedFarmer != null)
                {
                    var existingFarmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == updatedFarmer.FarmerId);
                    if (existingFarmer != null)
                    {
                        // Update the properties of the existing customer with the new values
                        existingFarmer.FirstName = updatedFarmer.FirstName;
                        existingFarmer.LastName = updatedFarmer.LastName;
                        existingFarmer.Email = updatedFarmer.Email;
                        existingFarmer.PhoneNumber = updatedFarmer.PhoneNumber;
                        existingFarmer.Address = updatedFarmer.Address;
                        existingFarmer.City = updatedFarmer.City;
                        existingFarmer.AboutMyFarm = updatedFarmer.AboutMyFarm;
                        if (image != null && image.Length > 0)
                        {
                            var fileName = Path.GetFileName(image.FileName);
                            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

                            using (var fileStream = new FileStream(imagePath, FileMode.Create))
                            {
                                image.CopyTo(fileStream);
                            }
                            // Set the image path in the farmer object
                            existingFarmer.Image = fileName;
                        }

                        _context.SaveChanges();
                        TempData["UpdateSuccess"] = true;

                        return RedirectToAction("PersonalDetails", new { farmerId = updatedFarmer.FarmerId });
                    }
                    else
                    {
                        // Handle the case when the customer is not found
                        return View("PersonalDetailsFarmer", updatedFarmer);
                    }
                }
                return View("FarmerLoginView");
            }
            else
            {
                // If the model is not valid, return the view with validation errors
                return RedirectToAction("GoToCustomerInterface", "Customer", new { isLoggedIn = false });
            }
        }

        public IActionResult PersonalDetails(int farmerId, string answer = null)
        {
            var farmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == farmerId);
            if (farmer != null)
            {
                if(answer != null)
                    farmer.AboutMyFarm = answer;

                return View("PersonalDetailsFarmer", farmer);
            }
            else
            {
                return RedirectToAction("GoToFarmerInterface", new { isLoggedIn, farmerId });
            }
        }

        public void ImageValidation(Product product, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }

                product.Image = fileName;
            }
        }

        [HttpPost]
        public IActionResult AddProduct(Product product, int farmerId, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var farmer = _context.Farmers
                .Include(f => f.Products) // Eager loading of Products navigation property
                .FirstOrDefault(f => f.FarmerId == farmerId);

                if (farmer != null && product != null)
                {
                    ImageValidation(product, image);
                    product.FarmerId = farmerId;
                    farmer.Products.Add(product);
                    _context.Products.Add(product);

                    _context.SaveChanges();
                    TempData["AddProductSuccess"] = $"Product {product.Name} has been added successfully.";

                    return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = true, farmerId });
                }
            }

            return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = true, farmerId });
        }

        public IActionResult DeleteProduct(int farmerId, int productId)
        {
            var farmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == farmerId);
            if (farmer != null)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    farmer.Products.Remove(product);
                    _context.Products.Remove(product);
                    _context.SaveChanges();

                    TempData["DeleteProductSuccess"] = $"Product {product.Name} has been deleted successfully.";
                }
            }

            return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = true, farmerId });
        }

        [HttpPost]
        public ActionResult EditProduct(int farmerId, int productId, Product updatedProduct, IFormFile image)
        {

            if (ModelState.IsValid)
            {
                var farmer = _context.Farmers
                    .Include(f => f.Products)
                    .FirstOrDefault(f => f.FarmerId == farmerId);

                if (farmer != null)
                {
                    var product = farmer.Products.FirstOrDefault(p => p.ProductId == productId);
                    if (product != null)
                    {

                        ImageValidation(product, image);

                        // Update the product details
                        product.Name = updatedProduct.Name;
                        product.Price = updatedProduct.Price;
                        product.Category = updatedProduct.Category;

                        // Update other properties as needed

                        _context.SaveChanges();

                        TempData["EditProductSuccess"] = $"Product {product.Name} has been updated successfully.";

                        return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = true, farmerId });
                    }
                }

                // If the product or farmer is not found, redirect back to the farmer interface
                TempData["ErrorMessage"] = "Product or farmer not found.";
                return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = true, farmerId });
            }

            // If the model is not valid, redirect back to the farmer interface
            TempData["ErrorMessage"] = "Invalid product data.";
            return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = true, farmerId });

        }

        public IActionResult FarmerOrders(int farmerId, int customerId)
        {
            var farmer = _context.Farmers
                            .Include(f => f.Products).
                             Include(f => f.Orders)               //Eager loading of Products navigation property
                            .FirstOrDefault(f => f.FarmerId == farmerId);
            if (farmer == null)
            {
                return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = false, farmerId });
            }
            var ordersWithOrderIdBiggerThan30 = farmer.Orders.Where(o => o.OrderId >= 30).ToList();
            // Extract unique customer IDs from the orders
            var customerIds = ordersWithOrderIdBiggerThan30.Select(o => o.CustomerId).Distinct();

            // Retrieve the corresponding customers from the database using the extracted IDs
            var customers = _context.Customers.Where(c => customerIds.Contains(c.CustomerId)).ToList();

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer != null)
            {
                ViewBag.Customer = customer;
                ViewBag.CustomerFullName = customer.FirstName + " " + customer.LastName;
                ViewBag.CustomerEmail = customer.Email;
                ViewBag.CustomerPhoneNumber = customer.PhoneNumber;
                ViewBag.CustomerAddress = customer.Address;
            }


            ViewBag.Orders = ordersWithOrderIdBiggerThan30;
            ViewBag.OrdersLength = ordersWithOrderIdBiggerThan30.Count();
            return View("FarmerOrders", farmer);
        }

        public IActionResult GetCustomerDetails(int farmerId, int customerId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            var farmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == farmerId);
            if (customer == null)
            {
                // Return a not found view or handle the situation when the customer is not found
                return NotFound();
            }
            ViewBag.Customer = customer; // You can use a ViewModel here if you have one
            return RedirectToAction("FarmerOrders", new { farmerId, customer.CustomerId });
        }



        public IActionResult ToggleOrderShipped(int orderId, int farmerId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.OrderShipped = !order.OrderShipped;
                if (order.OrderShipped == true)
                {
                    order.ShipDate = DateTime.Now.Date;
                }
                else
                {
                    order.ShipDate = DateTime.MinValue;
                }
                _context.SaveChanges();

                // Redirect back to the FarmerOrders view
                return RedirectToAction("FarmerOrders", "Farmer", new { farmerId = order.FarmerId });
            }
            else
            {
                return RedirectToAction("GoToFarmerInterface", new { isLoggedIn = false, farmerId });

            }
        }

        public double GetMonthlyEarnings(int farmerId)
        {
            var currentDate = DateTime.Now;
            var monthlyEarnings = _context.Orders
                                         .Where(o => o.FarmerId == farmerId && o.OrderId >= 30 &&
                                         o.OrderDate.Month == currentDate.Month && o.OrderDate.Year == currentDate.Year)
                                         .Sum(o => o.FinalPrice);

            return monthlyEarnings;
        }

        public double GetAnnualEarnings(int farmerId)
        {
            var currentDate = DateTime.Now;
            var annualEarnings = _context.Orders
                                        .Where(o => o.FarmerId == farmerId && o.OrderId >= 30 &&
                                        o.OrderDate.Year == currentDate.Year && o.FinalPrice > 0)
                                        .Sum(o => o.FinalPrice);
            return annualEarnings;
        }

        public double GetCompletedOrdersPercent(int farmerId)
        {
            var ordersWithOrderId30AndUp = _context.Orders
                                          .Where(o => o.FarmerId == farmerId && o.OrderId >= 30 && o.FinalPrice > 0);
            var totalOrdersCount = ordersWithOrderId30AndUp.Count();
            var completedOrdersCount = ordersWithOrderId30AndUp.Count(o => o.OrderShipped);

            double percentageCompleted = 0.0;
            if (totalOrdersCount > 0)
            {
                percentageCompleted = (completedOrdersCount * 100.0) / totalOrdersCount;
            }
            int floorPercentage = (int)Math.Floor(percentageCompleted);

            return floorPercentage;
        }
        public double GetAverageOrderPrice(int farmerId)
        {
            var ordersWithOrderId30AndUp = _context.Orders
                                      .Where(o => o.FarmerId == farmerId && o.OrderId >= 30 && o.FinalPrice > 0);

            double averageOrderPrice = ordersWithOrderId30AndUp.Average(o => o.FinalPrice);

            return averageOrderPrice;
        }
        public int GetAmountOfPendingOrders(int farmerId)
        {
            var ordersWithOrderId30AndUp = _context.Orders
                                          .Where(o => o.FarmerId == farmerId && o.OrderId >= 30 && o.FinalPrice > 0);
            var totalOrdersCount = ordersWithOrderId30AndUp.Count();
            var NOTcompletedOrdersCount = ordersWithOrderId30AndUp.Count(o => o.OrderShipped == false);

            return NOTcompletedOrdersCount;
        }
        public double GetAverageDaysToShip(int farmerId)
        {
            var ordersWithOrderId30AndUp = _context.Orders
            .Where(o => o.FarmerId == farmerId && o.OrderId >= 30 && o.OrderShipped && o.FinalPrice > 0);

            double averageShippingDuration = ordersWithOrderId30AndUp
                .AsEnumerable() // Fetch data from the database and perform the calculation in-memory
                .Select(o => (o.ShipDate - o.OrderDate).TotalDays)
                .Average();

            // Round the result to two decimal places
            return Math.Round(averageShippingDuration, 2);
        }
        public double GetAverageAmountProducts(int farmerId)
        {
            var ordersWithOrderId30AndUp = _context.Orders
                                 .Where(o => o.FarmerId == farmerId && o.OrderId >= 30 && o.FinalPrice > 0);

            int totalOrdersCount = ordersWithOrderId30AndUp.Count();

            if (totalOrdersCount == 0)
            {
                return 0; // Return 0 to avoid division by zero.
            }

            int totalProductsCount = ordersWithOrderId30AndUp.SelectMany(o => o.Products).Count();

            double averageAmountProducts = (double)totalProductsCount / totalOrdersCount;

            return Math.Round(averageAmountProducts, 2); // Round the average to 2 decimal places
        }
        public double GetPercentageOfVegetablesSold(int farmerId)
        {
            var ordersWithOrderId30AndUp = _context.Orders
            .Include(o => o.Products)  // Eager load the Products related to each order
            .Where(o => o.FarmerId == farmerId && o.OrderId >= 30 && o.FinalPrice > 0)
            .ToList();

            int totalProductsSold = 0;
            int totalVegetablesSold = 0;

            foreach (var order in ordersWithOrderId30AndUp)
            {
                foreach (var product in order.Products)
                {
                    if (product.Category == "Vegetable")
                    {
                        totalVegetablesSold++;
                    }
                    totalProductsSold++;
                }
            }

            double percentageOfVegetablesSold = 0;
            if (totalProductsSold > 0)
            {
                percentageOfVegetablesSold = (totalVegetablesSold * 100) / totalProductsSold;
            }

            return percentageOfVegetablesSold;
        }
        public double[] GetFarmerIncomeData(int farmerId)
        {
            var farmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == farmerId);

            double[] monthlyIncome = new double[11];

            var ordersWithOrderId30AndUp = _context.Orders
                .Where(o => o.FarmerId == farmerId && o.OrderId >= 30)
                .ToList();

            // Calculate the income for each month and update the income array
            foreach (var order in ordersWithOrderId30AndUp)
            {
                int monthIndex = order.OrderDate.Month - 1; // Month indices are 0-based
                monthlyIncome[monthIndex] += order.FinalPrice;
            }
            return monthlyIncome;
        }
        public List<double> GetPieFinalPrices(int farmerId)
        {
            var farmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == farmerId);

            var orders = _context.Orders.Where(o => o.FarmerId == farmerId && o.OrderId >= 30).ToList();

            int totalOrders = orders.Count;
            int orders0to50 = 0;
            int orders50to100 = 0;
            int orders100Plus = 0;

            foreach (var order in orders)
            {
                if (order.FinalPrice >= 0 && order.FinalPrice < 50)
                {
                    orders0to50++;
                }
                else if (order.FinalPrice >= 50 && order.FinalPrice < 100)
                {
                    orders50to100++;
                }
                else if (order.FinalPrice >= 100)
                {
                    orders100Plus++;
                }
            }

            double percent0to50 = (orders0to50 / (double)totalOrders) * 100;
            double percent50to100 = (orders50to100 / (double)totalOrders) * 100;
            double percent100Plus = (orders100Plus / (double)totalOrders) * 100;

            ViewBag.Percent0to50 = percent0to50;
            ViewBag.Percent50to100 = percent50to100;
            ViewBag.Percent100Plus = percent100Plus;

            var percentages = new List<double>
            {
                percent0to50,
                percent50to100,
                percent100Plus
            };
            return percentages;
        }
        public IActionResult FarmerDashboard(int farmerId)
        {
            var farmer = _context.Farmers.FirstOrDefault(f => f.FarmerId == farmerId);
            double monthlyEarnings = GetMonthlyEarnings(farmerId);
            double annualEarnings = GetAnnualEarnings(farmerId);
            double completedOrdersPercent = GetCompletedOrdersPercent(farmerId);
            int AmountPendingOrders = GetAmountOfPendingOrders(farmerId);
            double averageOrderPrice = GetAverageOrderPrice(farmerId);
            string formattedAverageOrderPrice = averageOrderPrice.ToString("F2");
            double averageAmountOfProducts = GetAverageAmountProducts(farmerId);
            string formattedAverageAmountProducts = averageAmountOfProducts.ToString("F2");
            double averageDaysToShip = GetAverageDaysToShip(farmerId);
            string formattedAverageDaysToShip = averageDaysToShip.ToString("F2");
            double percentageOfVegetablesSold = GetPercentageOfVegetablesSold(farmerId);
            double[] monthlyIncomeArray = GetFarmerIncomeData(farmerId);
            List<double> FinalPricePercentagesList = GetPieFinalPrices(farmerId);

            double _0To50 = FinalPricePercentagesList[0];
            double _50To100 = FinalPricePercentagesList[1];
            double _100Plus = FinalPricePercentagesList[2];


            ViewBag.MonthlyEarnings = monthlyEarnings;
            ViewBag.AnnualEarnings = annualEarnings;
            ViewBag.completedOrdersPercent = completedOrdersPercent;
            ViewBag.AmountPendingOrders = AmountPendingOrders;
            ViewBag.AverageOrderPrice = formattedAverageOrderPrice;
            ViewBag.AverageAmountOfProducts = formattedAverageAmountProducts;
            ViewBag.AverageDaysToShip = formattedAverageDaysToShip;
            ViewBag.PercentageOfVegetablesSold = percentageOfVegetablesSold;
            ViewBag.MonthlyIncomeArray = monthlyIncomeArray;

            ViewBag._0To50 = _0To50;
            ViewBag._50To100 = _50To100;
            ViewBag._100Plus = _100Plus;

            return View("AdminDashboard", farmer);
        }

    }
}
       


