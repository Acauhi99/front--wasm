using System.Collections.Generic;
using System.Linq;

namespace FrontWasm.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new();

        //.Sum() é um método LINQ que soma os valores de uma coleção.
        public decimal TotalValue => Items.Sum(item => item.TotalPrice);
    }
}
