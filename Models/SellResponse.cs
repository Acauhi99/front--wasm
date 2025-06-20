namespace front__wasm.Models
{
  // Status of sells
  public enum SellStatus
  {
    Pending = 0,
    Completed = 1,
    Cancelled = 2,
    Refunded = 3
  }

  // Response model from sell-related endpoints
  public class SellResponse
  {
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public string PackageId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? UserName { get; set; }
    public string? ServiceTitle { get; set; }
  }
}