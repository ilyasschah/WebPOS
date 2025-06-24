using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using WebPOS.Models;

namespace WebPOS.Pages.POS
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _ctx;

        public IndexModel(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public Business? Business { get; set; }
        public string BusinessType { get; set; } = "";

        public async Task OnGetAsync()
        {
            int? businessId = HttpContext.Session.GetInt32("BusinessId");

            if (businessId == null)
            {
                BusinessType = "";
                return;
            }

            Business = await _ctx.Businesses
                .Include(b => b.Template)
                .FirstOrDefaultAsync(b => b.BusinessId == businessId);

            BusinessType = Business?.Template?.TemplateName?.ToLower() ?? "";
        }
    }
}
