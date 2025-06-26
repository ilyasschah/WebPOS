using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebPOS.Data;
using WebPOS.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;

namespace WebPOS.Pages.Admin
{
    public class TablesSettingsModel : PageModel
    {
        private readonly AppDbContext _ctx;
        public TablesSettingsModel(AppDbContext ctx) => _ctx = ctx;

        public List<Table> Tables { get; set; }

        [BindProperty]
        public Table Table { get; set; }

        public async Task OnGetAsync()
        {
            Tables = await _ctx.Tables.OrderBy(t => t.Number).ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                Tables = await _ctx.Tables.OrderBy(t => t.Number).ToListAsync();
                return Page();
            }
            // Set default business id (replace 1 with your logic)
            Table.BusinessId = 1;
            // Set default status if not provided
            if (string.IsNullOrEmpty(Table.Status))
            {
                Table.Status = "available"; // Default status
            }

            Table.X = 0;
            Table.Y = 0;
            Table.Color = Table.Color ?? "#00bfff"; // Default color if not provided

            _ctx.Tables.Add(Table);
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid)
            {
                Tables = await _ctx.Tables.OrderBy(t => t.Number).ToListAsync();
                return Page();
            }
            var existingTable = await _ctx.Tables.FindAsync(Table.TableId);
            if (existingTable == null)
            {
                return NotFound();
            }
            existingTable.Name = Table.Name;
            existingTable.Status = Table.Status;
            existingTable.Color = Table.Color;
            existingTable.Shape = Table.Shape;
            existingTable.X = Table.X;
            existingTable.Y = Table.Y;
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var table = await _ctx.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            _ctx.Tables.Remove(table);
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            var table = await _ctx.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            table.Status = status;
            await _ctx.SaveChangesAsync();
            return RedirectToPage();
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostSavePositionsAsync()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var positions = System.Text.Json.JsonSerializer.Deserialize<List<TablePosition>>(body);

            if (positions != null)
            {
                foreach (var pos in positions)
                {
                    var table = await _ctx.Tables.FindAsync(Convert.ToInt32(pos.tableId));
                    if (table != null)
                    {
                        table.X = pos.x;
                        table.Y = pos.y;
                    }
                }
                await _ctx.SaveChangesAsync();
            }
            return new JsonResult(new { success = true });
        }

        public class TablePosition
        {
            public string tableId { get; set; }
            public int x { get; set; }
            public int y { get; set; }
        }
    }
}
