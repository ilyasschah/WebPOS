using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            var business = await _context.Businesses.FindAsync(BusinessInfo.BusinessId);
            if (business != null)
            {
                business.Name = BusinessInfo.Name;
                business.TemplateId = BusinessInfo.TemplateId;
            }
            else
            {
                _context.Businesses.Add(BusinessInfo);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
