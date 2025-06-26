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
            System.Diagnostics.Debug.WriteLine($"POSTED TemplateId: {BusinessInfo.TemplateId}");
            Templates = await _context.Templates.ToListAsync();
            TemplateSelectList = new SelectList(Templates, "TemplateId", "TemplateName");
            if (!ModelState.IsValid)
                return Page();

            // Check for duplicate name
            //var duplicate = await _context.Businesses
            //    .AnyAsync(b => b.Name == BusinessInfo.Name && b.BusinessId != BusinessInfo.BusinessId);
            //
            //if (duplicate)
            //{
            //    ModelState.AddModelError("BusinessInfo.Name", "A business with this name already exists.");
            //    return Page();
            //}

            // Always update the first (and only) business in your DB
            var existing = await _context.Businesses.FirstOrDefaultAsync();

            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(BusinessInfo);
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"UPDATED (force) TemplateId: {existing.TemplateId}");
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
