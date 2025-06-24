using System;
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

        [Column("userid")]
        public int UserId { get; set; }

        [Column("businessid")]
        public int BusinessId { get; set; }

        [Column("saledate")]
        public DateTime SaleDate { get; set; }

        [Column("totalamount")]
        public decimal TotalAmount { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("payment_type")]
        public int PaymentType { get; set; }
    }
}
