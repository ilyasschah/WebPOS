using System.Collections.Generic;

namespace WebPOS.Models
{
    public class POSFoodViewModel
    {
        public Business Business { get; set; } = null!;
        public List<Table> Tables { get; set; } = new();
        public string BusinessType { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new();
        public int? SelectedCategoryId { get; set; }
        public List<CartItem> Cart { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }
}