using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("sale_items")]
    public class SaleItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("sale_id")]
        public int SaleId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }
    }
}
