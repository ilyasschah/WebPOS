using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPOS.Models
{
    [Table("tables")]
    public class Table
    {
        [Column("tableid")]
        public int TableId { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("status")]
        public string Status { get; set; } = "";

        [Column("businessid")]
        public int BusinessId { get; set; }

        public Business Business { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        [NotMapped]
        public bool IsOccupied => Status == "occupied";
    }
}