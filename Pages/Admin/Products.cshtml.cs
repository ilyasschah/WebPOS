using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using WebPOS.Services;
using WebPOS.Models;

namespace WebPOS.Pages.Admin
{
    public class ProductsModel : PageModel
    {
        private readonly BusinessHelper _businessHelper;
        private readonly AppDbContext _context;

        public ProductsModel(AppDbContext context, BusinessHelper businessHelper)
        {
            _context = context;
            _businessHelper = businessHelper;
        }

        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        [BindProperty]
        public Product NewProduct { get; set; } = new();

        [BindProperty]
        public Product EditProduct { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? EditId { get; set; }

        public async Task OnGetAsync()
        {
            Products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            Categories = await _context.Categories.ToListAsync();

            if (EditId.HasValue)
            {
                var product = await _context.Products.FindAsync(EditId.Value);
                if (product != null)
                    EditProduct = product;
            }
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid) return Page();
            //product.Stock = product.Stock;
            NewProduct.BusinessId = _businessHelper.GetBusinessId();
            _context.Products.Add(NewProduct);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid) return RedirectToPage();

            var product = await _context.Products.FindAsync(EditProduct.ProductId);
            if (product == null) return NotFound();

            product.Name = EditProduct.Name;
            product.Price = EditProduct.Price;
            product.Stock = EditProduct.Stock;
            product.CategoryId = EditProduct.CategoryId;
            product.BusinessId = _businessHelper.GetBusinessId();
            
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
