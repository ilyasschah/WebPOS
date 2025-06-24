// File: /Pages/Admin/Inventory.cshtml.cs

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using WebPOS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebPOS.Services;  // <-- Add this for PermissionHelper

namespace WebPOS.Pages.Admin
{
    public class InventoryModel : PageModel
    {
        private readonly AppDbContext _ctx;
        private readonly PermissionHelper _permHelper; // <-- Add this

        public InventoryModel(AppDbContext ctx, PermissionHelper permHelper) // <-- Add permHelper param
        {
            _ctx = ctx;
            _permHelper = permHelper;
        }

        public List<Product> Products { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public List<Category> Categories { get; set; } = new();

        public bool CanUpdateStock { get; set; } // <-- Expose permission

        public async Task OnGetAsync()
        {
            Categories = await _ctx.Categories.OrderBy(c => c.Name).ToListAsync();
            var query = _ctx.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
                query = query.Where(p => p.Name.ToLower().Contains(Search.ToLower()));

            if (CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == CategoryId);

            Products = await query.OrderBy(p => p.Name).ToListAsync();

            // Check permission for updating stock
            CanUpdateStock = _permHelper.UserHasPermission("UpdateStock");
        }

        public async Task<IActionResult> OnPostUpdateStockAsync(int productId, int stock)
        {
            // Permission check before allowing update
            if (!_permHelper.UserHasPermission("UpdateStock"))
                return Forbid();

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
