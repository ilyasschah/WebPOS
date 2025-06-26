using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("orders")]
    public class Order
    {
        [Column("orderid")]
        public int OrderId { get; set; }

        [Column("tableid")]
        public int? TableId { get; set; }

        [Column("customerid")]
        public int? CustomerId { get; set; }

        [Column("businessid")]
        public int BusinessId { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("ordernumber")]
        public int OrderNumber { get; set; }

        [Column("order_type")]
        public string OrderType { get; set; } = "dinein";

        public List<OrderItem> OrderItems { get; set; } = new();
        public Customer? Customer { get; set; }
        public Business? Business { get; set; }
        public Table? Table { get; set; }
    }
}