using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("saleitems")] 
    public class SaleItem
    {
        [Key]
        [Column("saleitemid")]
        public int SaleItemId { get; set; }

        [Column("saleid")]
        public int SaleId { get; set; }

        [Column("productid")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("priceatsale")]
        public decimal PriceAtSale { get; set; }
    }
}
