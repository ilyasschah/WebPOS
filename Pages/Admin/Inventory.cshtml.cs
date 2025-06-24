using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using WebPOS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebPOS.Pages.Admin
{
    public class InventoryModel : PageModel
    {
        private readonly AppDbContext _ctx;
        public InventoryModel(AppDbContext ctx) => _ctx = ctx;

        public List<Product> Products { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public List<Category> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            Categories = await _ctx.Categories.OrderBy(c => c.Name).ToListAsync();
            var query = _ctx.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(p => p.Name.ToLower().Contains(Search.ToLower()));

            if (CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == CategoryId);

            Products = await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateStockAsync(int productId, int stock)
        {
            var product = await _ctx.Products.FindAsync(productId);
            if (product != null)
            {
                product.Stock = stock;
                await _ctx.SaveChangesAsync();
            }
            return RedirectToPage(new { Search, CategoryId });
        }
    }
}
