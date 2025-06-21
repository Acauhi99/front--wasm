namespace front__wasm.Models
{
  // Response específico para criação de vendas
  public class SellCreateResponse
  {
    public string Id { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public string PackageId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ServiceTitle { get; set; }

    // Campos específicos de pagamento
    public string PaymentMethod { get; set; } = string.Empty;
  }
}