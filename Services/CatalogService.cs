using System.Net.Http.Json;
using front__wasm.Models;


namespace front__wasm.Services
{
  public class CatalogService
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogService>? _logger;

    public CatalogService(HttpClient httpClient, ILogger<CatalogService>? logger = null)
    {
      _httpClient = httpClient;
      _logger = logger;
    }

    public async Task<IEnumerable<CatalogServiceDetails>> GetServicesAsync()
    {
      try
      {
        var servicesDict = await _httpClient.GetFromJsonAsync<Dictionary<string, ServiceDetails>>("data/services-details.json");

        if (servicesDict == null)
          return new List<CatalogServiceDetails>();

        // Convert dictionary entries to CatalogServiceDetails objects
        var services = servicesDict.Select(kvp => new CatalogServiceDetails
        {
          Title = kvp.Value.Title,
          Description = kvp.Value.Description,
          DetailsUrl = $"/{kvp.Key}",
          ImageUrl = kvp.Key switch
          {
            "gold" => "images/gold-services.png",
            "dungeons" => "images/mythic-plus.png",
            "raids" => "images/raid-boosting.png",
            "pvp" => "images/pvp-services.png",
            "itens" => "images/special-itens.png",
            _ => ""
          },

          Price = kvp.Value.Packages.FirstOrDefault(p => p.IsPopular)?.Price ??
                  kvp.Value.Packages.FirstOrDefault()?.Price ?? ""
        }).ToList();

        _logger?.LogInformation($"Loaded {services.Count} services");
        return services;
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Erro ao carregar serviços: {Message}", ex.Message);
        Console.WriteLine($"Error loading services: {ex.Message}");
        return new List<CatalogServiceDetails>();
      }
    }
  }
}
