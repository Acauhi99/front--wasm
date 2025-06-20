namespace front__wasm.Models
{
  public class Service
  {
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FeaturesTitle { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public string DetailsTitle { get; set; } = string.Empty;
    public List<string> Details { get; set; } = new();
    public string PackagesTitle { get; set; } = string.Empty;
    public List<ServicePackage> Packages { get; set; } = new();
  }

  public class ServicePackage
  {
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public bool IsPopular { get; set; } = false;
  }
}