using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WebPOS.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebPOS.Pages.Admin
{
    public class InventoryPdfModel : PageModel
    {
        private readonly AppDbContext _ctx;
        public InventoryPdfModel(AppDbContext ctx) => _ctx = ctx;

        public async Task<IActionResult> OnGetAsync()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var products = await _ctx.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text("Inventory Report").FontSize(20).SemiBold().AlignCenter();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Product");
                            header.Cell().Element(CellStyle).Text("Category");
                            header.Cell().Element(CellStyle).Text("Stock");
                        });

                        foreach (var p in products)
                        {
                            table.Cell().Element(CellStyle).Text(p.Name);
                            table.Cell().Element(CellStyle).Text(p.Category?.Name ?? "");
                            table.Cell().Element(CellStyle).Text(p.Stock.ToString());
                        }
                        static IContainer CellStyle(IContainer container) =>
                            container.DefaultTextStyle(x => x.SemiBold()).Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });
                    page.Footer().AlignCenter().Text($"Generated at {System.DateTime.Now}");
                });
            });

            var pdfBytes = doc.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Inventory_{System.DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }
    }
}
