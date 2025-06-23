using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<WebPOS.Services.BusinessHelper>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();


app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
