using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using WebPOS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace WebPOS.Pages.Admin
{
    public class SalesReportModel : PageModel
    {
        private readonly AppDbContext _ctx;
        public SalesReportModel(AppDbContext ctx) => _ctx = ctx;

        public List<Sale> Sales { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public async Task OnGetAsync(DateTime? from = null, DateTime? to = null)
        {
            From = from?.Date != null ? DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc) : DateTime.UtcNow.Date;
            To = to?.Date != null ? DateTime.SpecifyKind(to.Value.Date, DateTimeKind.Utc) : DateTime.UtcNow.Date;

            var salesQuery = _ctx.Sales.AsQueryable();

            if (From.HasValue)
                salesQuery = salesQuery.Where(s => s.SaleDate >= From.Value);
            if (To.HasValue)
                salesQuery = salesQuery.Where(s => s.SaleDate < To.Value.AddDays(1));

            Sales = await salesQuery
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            TotalAmount = Sales.Sum(s => s.TotalAmount);
        }
    }
}
