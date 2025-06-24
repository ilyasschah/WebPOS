using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("businesses")]
    public class Business
    {
        [Key]
        [Column("businessid")]
        public int BusinessId { get; set; }
        
        [Required]
        [Column("name")]
        [StringLength(128)]
        public string Name { get; set; } = "";

        [Column("templateid")]
        public int TemplateId { get; set; }

        [ForeignKey("TemplateId")]
        public Template? Template { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
