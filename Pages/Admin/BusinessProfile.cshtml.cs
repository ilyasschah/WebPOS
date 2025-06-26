using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using WebPOS.Models;

namespace WebPOS.Pages.Admin
{
    public class BusinessProfileModel : PageModel
    {
        private readonly AppDbContext _context;

        public BusinessProfileModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Business BusinessInfo { get; set; } = new();

        public List<Template> Templates { get; set; } = new();
        public SelectList TemplateSelectList { get; set; }
        public async Task OnGetAsync()
        {
            // Load first business (you can extend this to support multi-business later)
            BusinessInfo = await _context.Businesses
                .Include(b => b.Template)
                .FirstOrDefaultAsync() ?? new Business();

            Templates = await _context.Templates.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            // Always update the first (and only) business in your DB
            var existing = await _context.Businesses.FindAsync(BusinessInfo.BusinessId);
            if (existing != null)
            {
                existing.Name = BusinessInfo.Name;
                existing.TemplateId = BusinessInfo.TemplateId;
                await _context.SaveChangesAsync();
            }
            else
            {
                // If there is really no business (first-time install), add it
                _context.Businesses.Add(BusinessInfo);
                await _context.SaveChangesAsync();
            }

            // For confirmation/debug: fetch again and output
            var check = await _context.Businesses.FirstOrDefaultAsync();
            System.Diagnostics.Debug.WriteLine($"Saved! Name: {check?.Name}, TemplateId: {check?.TemplateId}");

            return RedirectToPage("/Admin/BusinessProfile");

        }
    }
}
