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
        [Column("number")]
        public int Number { get; set; }

        [Column("color")]
        public string Color { get; set; }

        [Column("shape")]
        public string Shape { get; set; }

        [Column("x")]
        public int X { get; set; } // position X

        [Column("y")]
        public int Y { get; set; } // position Y

        public Business Business { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        [NotMapped]
        public bool IsOccupied => Status == "occupied";
    }
}