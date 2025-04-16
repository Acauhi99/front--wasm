using System;

namespace FrontWasm.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
        public int Quantity { get; set; }

        
        public decimal TotalPrice => (Service?.Price ?? 0) * Quantity;
    }
}
