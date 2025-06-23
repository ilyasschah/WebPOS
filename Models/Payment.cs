using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("payments")]
    public class Payment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("method")]
        public string Method { get; set; } = "";

        [Column("amount")]
        public decimal Amount { get; set; }
    }
}
