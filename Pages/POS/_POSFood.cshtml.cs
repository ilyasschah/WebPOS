using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebPOS.Data;
using WebPOS.Models;

namespace WebPOS.Pages.POS
{
    public class FoodModel : PageModel
    {
        private readonly AppDbContext _ctx;
        public FoodModel(AppDbContext ctx) => _ctx = ctx;

        public List<Table> Tables { get; set; } = new();
        public List<Order> Orders { get; set; } = new();

        public async Task OnGetAsync()
        {
            Tables = await _ctx.Tables.ToListAsync();
            // Only load non-completed orders
            Orders = await _ctx.Orders
                .Where(o => o.Status == "open")
                .ToListAsync();
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostUpdateTableStatusAsync()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var data = JsonSerializer.Deserialize<TableStatusRequest>(body);

            if (data == null || data.tableId == 0)
                return new JsonResult(new { success = false });

            var table = await _ctx.Tables.FindAsync(data.tableId);
            if (table == null) return NotFound();
            table.Status = data.status;
            await _ctx.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostCreateOrderAsync()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var data = JsonSerializer.Deserialize<CreateOrderRequest>(body);

            if (data == null || data.tableId == 0)
                return new JsonResult(new { success = false, error = "Invalid data" });

            // Check if there's already an open order
            var openOrder = await _ctx.Orders
                .FirstOrDefaultAsync(o => o.TableId == data.tableId && o.Status == "open");

            if (openOrder != null)
            {
                return new JsonResult(new { success = true, orderId = openOrder.OrderId, alreadyExists = true });
            }

            // Create new order
            var order = new Order
            {
                TableId = data.tableId,
                Status = "open",
                CreatedAt = System.DateTime.UtcNow,
                // Add any other required fields here!
            };
            _ctx.Orders.Add(order);

            // Mark table as occupied
            var table = await _ctx.Tables.FindAsync(data.tableId);
            if (table != null)
                table.Status = "occupied";

            await _ctx.SaveChangesAsync();

            return new JsonResult(new { success = true, orderId = order.OrderId, alreadyExists = false });
        }

        public class TableStatusRequest
        {
            public int tableId { get; set; }
            public string status { get; set; }
        }

        public class CreateOrderRequest
        {
            public int tableId { get; set; }
        }
    }
}
