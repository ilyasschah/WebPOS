using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using WebPOS.Models;
using WebPOS.Services;


namespace WebPOS.Pages.Admin
{
    public class CategoriesModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly BusinessHelper _businessHelper;
        public CategoriesModel(AppDbContext context, BusinessHelper businessHelper)
        {
            _context = context;
            _businessHelper = businessHelper;
        }

        public List<Category> Categories { get; set; } = new();

        [BindProperty]
        public Category NewCategory { get; set; } = new();

        public async Task OnGetAsync()
        {
            Categories = await _context.Categories.ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            NewCategory.BusinessId = _businessHelper.GetBusinessId();
            _context.Categories.Add(NewCategory);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat != null)
            {
                _context.Categories.Remove(cat);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
