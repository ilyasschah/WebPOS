using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("templates")]
    public class Template
    {
        [Key]
        [Column("templateid")]
        public int TemplateId { get; set; }

        [Column("templatename")]
        public string TemplateName { get; set; } = "";
        //public string? Description { get; set; }
        public ICollection<Business>? Businesses { get; set; }
    }
}
