using System;
using System.Collections.Generic;

namespace WebPOS.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int? TableId { get; set; }
        public int? CustomerId { get; set; }
        public int BusinessId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int OrderNumber { get; set; }

        // Navigation properties if needed
        public Customer? Customer { get; set; }
        public Business? Business { get; set; }
        public Table? Table { get; set; }
    }
}
