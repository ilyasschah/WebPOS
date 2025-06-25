using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using WebPOS.Models;
var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<WebPOS.Services.BusinessHelper>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<WebPOS.Services.PermissionHelper>();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24); // Max default, will be shortened per user later
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();


app.UseRouting();

app.UseSession();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";

    bool isBusinessSetup = path.StartsWith("/setup/businesssetup");
    bool isAdminSetup = path.StartsWith("/setup/adminsetup");
    bool isLoginPage = path.StartsWith("/account/login");
    bool isStatic = path.StartsWith("/css") || path.StartsWith("/js") ||
                           path.StartsWith("/lib") || path.StartsWith("/images") ||
                           path.StartsWith("/favicon");

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<WebPOS.Data.AppDbContext>();

        // Business setup if needed
        if (!isBusinessSetup && !isStatic && !isAdminSetup && !isLoginPage)
        {
            if (!db.Businesses.Any())
            {
                context.Response.Redirect("/Setup/BusinessSetup");
                return;
            }

            // Admin setup if needed
            if (!db.Users.Any())
            {
                context.Response.Redirect("/Setup/AdminSetup");
                return;
            }
        }
        // Check if "Default" customer exists
        //if (!db.Customers.Any(c => c.Name == "Default"))
        //{
        //    db.Customers.Add(new Customer
        //    {
        //        Name = "Default"
        //        // Add any required fields with default values
        //    });
        //    db.SaveChanges();
        //}
        // Require login on all protected pages
        bool authenticated = context.Session.GetInt32("UserId") != null;
        if (!isStatic && !isBusinessSetup && !isAdminSetup && !isLoginPage)
        {
            if (!authenticated)
            {
                context.Response.Redirect("/Account/Login");
                return;
            }
        }
        // If logged in and tries to go to /Account/Login, redirect to home
        if (isLoginPage && authenticated)
        {
            context.Response.Redirect("/Index");
            return;
        }
    }
    await next.Invoke();
});



app.UseAuthorization();

app.MapRazorPages();

app.Run();
