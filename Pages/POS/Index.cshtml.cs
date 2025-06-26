using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebPOS.Data;
using WebPOS.Models;
using System;
using System.Linq;
using System.IO;

namespace WebPOS.Pages.POS
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _ctx;
        public IndexModel(AppDbContext ctx) => _ctx = ctx;

        public Business? Business { get; set; }
        public string BusinessType { get; set; } = string.Empty;
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();
        public int? SelectedCategoryId { get; set; }
        public List<Table> Tables { get; set; }
        public List<Order> Orders { get; set; }
        public List<CartItem> Cart { get; set; } = new();

        public async Task OnGetAsync(int? categoryId = null)
        {
            int? businessId = HttpContext.Session.GetInt32("BusinessId");

            if (businessId == null)
            {
                BusinessType = string.Empty;
                return;
            }

            Business = await _ctx.Businesses
                .Include(b => b.Template)
                .FirstOrDefaultAsync(b => b.BusinessId == businessId);

            BusinessType = Business?.Template?.TemplateName?.ToLower() ?? string.Empty;
            Tables = _ctx.Tables.Where(t => t.BusinessId == Business.BusinessId).ToList();
            Orders = _ctx.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .Where(o => o.BusinessId == Business.BusinessId && o.Status != "completed")
                .ToList();

            Categories = await _ctx.Categories
                .Where(c => c.BusinessId == businessId)
                .OrderBy(c => c.Name)
                .ToListAsync();

            SelectedCategoryId = categoryId;

            Products = await _ctx.Products
                .Where(p => p.BusinessId == businessId &&
                    (categoryId == null || p.CategoryId == categoryId))
                .OrderBy(p => p.Name)
                .ToListAsync();
            Cart = GetCart();
        }

        private const string CartSessionKey = "POS_Cart";

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();
            return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = System.Text.Json.JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int? categoryId = null)
        {
            int? businessId = HttpContext.Session.GetInt32("BusinessId");
            if (businessId == null)
                return RedirectToPage("/Account/Login");

            var product = await _ctx.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.BusinessId == businessId);
            if (product == null)
                return RedirectToPage();

            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }
            SaveCart(cart);

            // Redirect to same page with filter
            if (categoryId != null)
                return RedirectToPage(new { categoryId });
            return RedirectToPage();
        }

        public IActionResult OnPostIncreaseQuantity(int productId, int? categoryId = null)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity++;
                SaveCart(cart);
            }
            if (categoryId != null)
                return RedirectToPage(new { categoryId });
            return RedirectToPage();
        }

        public IActionResult OnPostDecreaseQuantity(int productId, int? categoryId = null)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    cart.Remove(item);
                SaveCart(cart);
            }
            if (categoryId != null)
                return RedirectToPage(new { categoryId });
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveFromCart(int productId, int? categoryId = null)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            if (categoryId != null)
                return RedirectToPage(new { categoryId });
            return RedirectToPage();
        }

        public IActionResult OnPostClearCart(int? categoryId = null)
        {
            SaveCart(new List<CartItem>());
            if (categoryId != null)
                return RedirectToPage(new { categoryId });
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPayAsync(int? categoryId = null)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Account/Login");

            var cart = GetCart();
            if (cart == null || !cart.Any())
                return RedirectToPage();

            decimal total = cart.Sum(i => i.Subtotal);
            int? businessId = HttpContext.Session.GetInt32("BusinessId");
            if (businessId == null)
                return RedirectToPage("/Account/Login");

            // 1. Daily order number logic
            var today = DateTime.UtcNow.Date;
            int maxOrderNumberToday = await _ctx.Orders
                .Where(o => o.CreatedAt.Date == today)
                .Select(o => o.OrderNumber)
                .DefaultIfEmpty(0)
                .MaxAsync();
            int newOrderNumber = maxOrderNumberToday + 1;

            // 2. Handle default customer
            var defaultCustomer = await _ctx.Customers.FirstOrDefaultAsync(c => c.Name == "Default");

            // 3. Create the Order and save it
            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                OrderNumber = newOrderNumber,
                BusinessId = businessId.Value,
                CustomerId = defaultCustomer != null ? defaultCustomer.Id : (int?)null
                // TODO: set TableId if using tables (add it here)
            };
            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync();

            // 4. Create the Sale linked to the Order
            var sale = new Sale
            {
                BusinessId = businessId.Value,
                SaleDate = DateTime.UtcNow,
                TotalAmount = total,
                UserId = userId.Value,
                OrderId = order.OrderId,      // Use the new Order you just saved
                TableId = order.TableId,      // Use if you have TableId, otherwise keep as null
                CustomerId = order.CustomerId,
                PaymentType = 1
            };

            _ctx.Sales.Add(sale);
            await _ctx.SaveChangesAsync();

            // 5. Create sale items
            foreach (var item in cart)
            {
                var saleItem = new SaleItem
                {
                    SaleId = sale.SaleId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtSale = item.Price
                };
                _ctx.SaleItems.Add(saleItem);
            }
            await _ctx.SaveChangesAsync();

            // 6. Update product stock
            foreach (var item in cart)
            {
                var product = await _ctx.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                }
            }
            await _ctx.SaveChangesAsync();

            SaveCart(new List<CartItem>()); // clear cart

            if (categoryId != null)
                return RedirectToPage(new { categoryId });
            return RedirectToPage();
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostCreateOrderAsync()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<CreateOrderRequest>(body);

                if (data == null || data.tableId == 0)
                    return new JsonResult(new { success = false, error = "Invalid data" });

                int? businessId = HttpContext.Session.GetInt32("BusinessId");
                if (businessId == null)
                    return new JsonResult(new { success = false, error = "No business" });

                // Check if open order already exists for this table
                var existingOrder = await _ctx.Orders
                    .Where(o => o.TableId == data.tableId && o.Status == "open" && o.BusinessId == businessId)
                    .FirstOrDefaultAsync();

                if (existingOrder != null)
                {
                    return new JsonResult(new { success = true, orderId = existingOrder.OrderId });
                }

                // Create new order
                var order = new Order
                {
                    TableId = data.tableId,
                    Status = "open",
                    BusinessId = businessId.Value,
                    CreatedAt = DateTime.UtcNow,
                    OrderNumber = await GetNextOrderNumberAsync()
                };

                _ctx.Orders.Add(order);

                // Mark the table as occupied
                var table = await _ctx.Tables.FindAsync(data.tableId);
                if (table != null)
                    table.Status = "occupied";

                await _ctx.SaveChangesAsync();

                return new JsonResult(new { success = true, orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }

        public class CreateOrderRequest
        {
            public int tableId { get; set; }
        }

        // Helper to generate next order number (daily)
        private async Task<int> GetNextOrderNumberAsync()
        {
            var today = DateTime.UtcNow.Date;
            int maxOrderNumberToday = await _ctx.Orders
                .Where(o => o.CreatedAt.Date == today)
                .Select(o => o.OrderNumber)
                .DefaultIfEmpty(0)
                .MaxAsync();
            return maxOrderNumberToday + 1;
        }
    }
}
