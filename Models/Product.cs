using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("productid")]
        public int ProductId { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("price")]
        public decimal Price { get; set; }

        [Column("stock")]
        public int Stock { get; set; }

        [Column("categoryid")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Column("businessid")]
        public int? BusinessId { get; set; }
    }
}
