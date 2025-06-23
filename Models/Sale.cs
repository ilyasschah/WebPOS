using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("sales")]
    public class Sale
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("payment_id")]
        public int PaymentId { get; set; }
    }
}
