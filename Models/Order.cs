using System;
using System.Collections.Generic;

namespace WebPOS.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int? TableId { get; set; }
        public Table Table { get; set; }

        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int? BusinessId { get; set; }
        public Business Business { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
