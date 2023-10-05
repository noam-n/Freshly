using FarmersPlatform.Data;
using FarmersPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Product = FarmersPlatform.Models.Product;

namespace FarmersPlatform.Controllers
{
    public class CheckoutController : Controller
    {
        Basket _basket = Basket.GetInstance();
        private FarmContext _context;

        public CheckoutController(FarmContext context)
        {
            _context = context;
        }
        public IActionResult Checkout(int farmerId, int customerId, string? orderRequestMessage)
        {
            var products = _basket.GetProducts();
            if (products != null && products.Count() > 0)
            {
                var domain = "http://localhost:5152/";
                var succuessUrl = domain + $"Checkout/Success?customerId={customerId}&farmerId={farmerId}";

                if (orderRequestMessage != null)
                {
                    succuessUrl = domain + $"Checkout/Success?customerId={customerId}&farmerId={farmerId}&requestMessage={orderRequestMessage}";
                }
                var options = new SessionCreateOptions
                {
                    SuccessUrl = succuessUrl,
                    CancelUrl = domain + $"Checkout/CancelTransaction?customerId={customerId}",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    PaymentMethodTypes = new List<string> { "card" },

                };


                foreach (var item in products)
                {
                    var sessionListItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),

                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Name,
                            }
                        },
                        Quantity = item.Quantity,
                    };
                    options.LineItems.Add(sessionListItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);

            }

            return new StatusCodeResult(303);
        }

        public IActionResult Success(int customerId, int farmerId, string? requestMessage = null)
        {
            TempData["OrderConfirmationMessage"] = "Congratulations! Your order has been successfully processed and the farmer has been notified. We're working hard to bring your fresh produce to your doorstep.";
            if (requestMessage != null)
                CreateOrder(customerId, farmerId, requestMessage);
            else
                CreateOrder(customerId, farmerId);

            return RedirectToAction("GoToCustomerInterface", "Customer", new { isLoggedIn = true, customerId });
        }


        public IActionResult CancelTransaction(int customerId)
        {
            return RedirectToAction("GoToCustomerInterface", "Customer", new { isLoggedIn = true, customerId });
        }

        public void CreateOrder(int customerId, int farmerId, string? requestMessage = null)
        {
            var order = new Order
            {
                CustomerId = customerId,
                FarmerId = farmerId,
                OrderDate = DateTime.Now.Date,
                FinalPrice = _basket.GetTotalPrice(),
            };

            if (requestMessage != null)
            {
                order.RequestMessage = requestMessage;
            }

            _context.Orders.Add(order);

            var productsFromBasket = _basket.GetProducts();
            var productsToAdd = new List<Product>();

            foreach (var product in productsFromBasket)
            {
                var existingProduct = order.Products.FirstOrDefault(p => p.Name == product.Name);

                if (existingProduct != null)
                {
                    // If the product already exists in the order, update its quantity
                    existingProduct.Quantity += product.Quantity;
                }
                else
                {
                    // If the product doesn't exist in the order, add it to the list of products to be added
                    productsToAdd.Add(new Product
                    {
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        FarmerId = farmerId,
                        Category = product.Category,
                        Image = product.Image,
                    });

                }
            }

            foreach (var newProduct in productsToAdd)
            {
                _context.Products.Add(newProduct);
                order.Products.Add(newProduct);
            }

            _context.SaveChanges();
        }

        private void UpdateProductQuantity(int productId, int newQuantity)
        {
            var product = _basket.GetProducts().FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                product.Quantity = newQuantity;
            }
        }

        private double CalculateTotalPrice()
        {
            double totalPrice = 0;
            foreach (var item in _basket.GetProducts())
            {
                totalPrice += item.Price * item.Quantity;
            }
            return totalPrice;
        }

        public IActionResult GoToPreCheckout(int farmerId, int customerId, string? OrderRequestMessage)
        {
            ViewBag.numberOfItems = _basket.GetNumberOfItems();
            ViewBag.Products = _basket.GetProducts();
            ViewBag.OrderTotal = _basket.GetTotalPrice();
            ViewBag.FarmerId = farmerId;

            if (OrderRequestMessage != null)
            {
                ViewBag.OrderRequestMessage = OrderRequestMessage;
            }
            var customer = _context.Customers.Where(c => c.CustomerId == customerId).FirstOrDefault();
            return View("~/Views/Customer/PreCheckoutView.cshtml", customer);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int newQuantity, int farmerId, int customerId)
        {
            UpdateProductQuantity(productId, newQuantity);
            double totalPrice = CalculateTotalPrice();
            _basket.UpdateTotalPrice(totalPrice);

            return RedirectToAction("GoToPreCheckout", new { farmerId, customerId });
        }

        public IActionResult UpdateCustomerDetails(string firstName, string lastName, string email, string phoneNumber, string address, string orderRequestMessage, int customerId, int farmerId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customer != null)
            {
                // Update customer's properties
                customer.FirstName = firstName;
                customer.LastName = lastName;
                customer.Email = email;
                customer.PhoneNumber = phoneNumber;
                customer.Address = address;

                // Save changes
                _context.SaveChanges();

                string OrderRequestMessage = orderRequestMessage;


                return RedirectToAction("GoToPreCheckout", new { farmerId, customerId, OrderRequestMessage });
            }
            return RedirectToAction("GoToCustomerInterface", "Customer", new { isLoggedIn = true, customerId });
        }
    }
}


