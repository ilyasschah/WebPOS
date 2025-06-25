using Microsoft.EntityFrameworkCore;
using WebPOS.Models;

public List<Table> Tables { get; set; }
public List<Order> Orders { get; set; }
// Add these in your OnGet handler
Tables = _context.Tables.Where(t => t.BusinessId == Model.Business.Id).ToList();
Orders = _context.Orders
    .Include(o => o.Table)
    .Include(o => o.OrderItems)
    .Where(o => o.BusinessId == Model.Business.Id && !o.IsCompleted)
    .ToList();
