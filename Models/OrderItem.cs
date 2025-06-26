using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("orderitem")]
    public class OrderItem
    {
        [Column("orderitemid")]
        public int OrderItemId { get; set; }

        [Column("orderid")]
        public int OrderId { get; set; }

        [Column("productid")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
