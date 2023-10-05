using FarmersPlatform.Data;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]!;
builder.Services.AddDbContext<FarmContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllersWithViews();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<FarmContext>();
    ctx.Database.EnsureCreated();
}

app.UseStaticFiles();
app.UseRouting();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.MapControllerRoute("Default", "{controller=Home}/{action=ShowHomePage}/{id?}");

app.MapControllerRoute(
    name: "checkoutSuccess",
    pattern: "checkout/success",
    defaults: new { controller = "Checkout", action = "Success" }
);

app.Run();
