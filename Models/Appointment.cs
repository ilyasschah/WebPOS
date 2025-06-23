using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("appointments")]
    public class Appointment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("service")]
        public string Service { get; set; } = "";

        [Column("user_id")]
        public int UserId { get; set; }
    }
}
