using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebPOS.Data;
using WebPOS.Models;

namespace WebPOS.Pages.POS
{
    public class IndexModel(AppDbContext ctx) : PageModel
    {
        private readonly AppDbContext _ctx = ctx;
        public Business? Business { get; set; }
        public string BusinessType { get; set; } = string.Empty;
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();
        public int? SelectedCategoryId { get; set; }
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
            // 1. Create a new sale
            var sale = new Sale
            {
                BusinessId = businessId.Value,
                SaleDate = DateTime.UtcNow,
                TotalAmount = total,
                UserId = userId.Value,
                // Fill these as needed (for now, zero or null)
                CustomerId = null,    // or assign a real customer if you have one
                PaymentType = 1         // 1 = cash? You can improve this later.
            };

            _ctx.Sales.Add(sale);
            await _ctx.SaveChangesAsync(); // Sale.Id will now be filled

            // 2. Create sale items
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

            foreach (var item in cart) // Moved this block to ensure it is reachable
            {
                var product = await _ctx.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity; // This allows negative stock (as requested)
                }
            }
            await _ctx.SaveChangesAsync();

            SaveCart(new List<CartItem>()); // clear cart

            // Redirect or show receipt as needed
            if (categoryId != null)
                return RedirectToPage(new { categoryId });
            return RedirectToPage();
        }
    }
}
