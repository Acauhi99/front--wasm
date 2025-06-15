namespace front__wasm.Models
{
  // Modelo principal que representa a estrutura JSON da API
  public class Service
  {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FeaturesTitle { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public string DetailsTitle { get; set; } = string.Empty;
    public List<string> Details { get; set; } = new();
    public string PackagesTitle { get; set; } = string.Empty;
    public List<ServicePackage> Packages { get; set; } = new();
  }

  // Parte do modelo principal
  public class ServicePackage
  {
    public string Name { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public bool IsPopular { get; set; } = false;
  }
}