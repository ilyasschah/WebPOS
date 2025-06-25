using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("sales")]
    public class Sale
    {
        [Key]
        [Column("saleid")]
        public int SaleId { get; set; }

        [Column("userid")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("businessid")]
        public int BusinessId { get; set; }
        public Business Business { get; set; }

        [Column("saledate")]
        public DateTime SaleDate { get; set; }

        [Column("totalamount")]
        public decimal TotalAmount { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Column("payment_type")]
        public int PaymentType { get; set; }
        [Column("order_id")]
        public int? OrderId { get; set; }
        public Order Order { get; set; }
        [Column("table_id")]
        public int? TableId { get; set; }
        public Table Table { get; set; }
    }
}
