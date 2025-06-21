namespace front__wasm.Models
{
  public class SellRequest
  {
    public List<SellItemRequest> Items { get; set; } = new List<SellItemRequest>();
    public decimal Subtotal { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = "PIX";
  }

  public class SellItemRequest
  {
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
  }
}