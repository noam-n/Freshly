using FarmersPlatform.Data;
using FarmersPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;
using Stripe;

namespace FarmersPlatform.Controllers
{
    public class HomeController : Controller
    {
        private FarmContext _context;

        public HomeController(FarmContext context)
        {
            _context = context;
        }   

        public IActionResult ShowHomePage()
        {
            return View("HomeLandingPage");
        }

        public IActionResult ShowLoginPage(bool isFarmer)
        {
            if (isFarmer)
            {
                return View("~/Views/Farmer/FarmerLoginView.cshtml");
            }
            else
            {
                return View("~/Views/Customer/CustomerLoginView.cshtml");
            }
        }

        public IActionResult ShowSignupPage(bool isFarmer)
        {
            if (isFarmer)
            {
                // Logic for farmer signup
                return View("~/Views/Farmer/FarmerSignupView.cshtml");
            }
            else
            {
                // Logic for regular customer signup
                return View("~/Views/Customer/CustomerSignupView.cshtml");
            }
        }


    }
}
