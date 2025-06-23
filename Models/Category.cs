using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("categoryid")]
        public int CategoryId { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("businessid")]
        public int? BusinessId { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
