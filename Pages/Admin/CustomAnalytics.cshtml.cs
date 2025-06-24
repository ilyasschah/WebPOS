using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPOS.Pages.Admin
{
    public class CustomAnalyticsModel : PageModel
    {
        private readonly AppDbContext _ctx;
        public CustomAnalyticsModel(AppDbContext ctx) => _ctx = ctx;

        // KPI properties
        public int TotalSalesCount { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public List<(DateTime Day, decimal Total)> SalesByDay { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Last 30 days sales for chart
            var fromDate = DateTime.UtcNow.AddDays(-29).Date;
            var toDate = DateTime.UtcNow.Date;

            // KPIs
            TotalSalesCount = await _ctx.Sales.CountAsync();
            TotalSalesAmount = await _ctx.Sales.SumAsync(s => (decimal?)s.TotalAmount ?? 0);
            TotalProducts = await _ctx.Products.CountAsync();
            LowStockProducts = await _ctx.Products.CountAsync(p => p.Stock < 5);

            // Sales by day (for area/bar chart)
            var salesList = await _ctx.Sales
                .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate)
                .ToListAsync(); // <-- async fetch

            SalesByDay = salesList
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => (g.Key, g.Sum(s => s.TotalAmount)))
                .OrderBy(pair => pair.Key)
                .ToList();

        }
    }
}
