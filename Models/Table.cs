namespace WebPOS.Models
{
    public class Table
    {
        public int TableId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } // e.g., Free, Occupied, Reserved
        public int BusinessId { get; set; }
        public Business Business { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
