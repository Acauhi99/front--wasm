using System.Net.Http.Json;
using front__wasm.Models;

namespace front__wasm.Services
{
  public class ServiceManager
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceManager>? _logger;
    private Dictionary<string, Service>? _cachedServices;

    public ServiceManager(HttpClient httpClient, ILogger<ServiceManager>? logger = null)
    {
      _httpClient = httpClient;
      _logger = logger;
    }

    // Carrega todos os serviços da API
    public async Task<Dictionary<string, Service>> GetAllServicesAsync()
    {
      if (_cachedServices != null)
        return _cachedServices;

      try
      {
        var services = await _httpClient.GetFromJsonAsync<Dictionary<string, Service>>("data/services-details.json");
        _cachedServices = services ?? new Dictionary<string, Service>();
        return _cachedServices;
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Erro ao carregar serviços: {Message}", ex.Message);
        return new Dictionary<string, Service>();
      }
    }

    // Obtém um serviço específico pelo ID
    public async Task<ServiceDetailsViewModel?> GetServiceDetailsAsync(string serviceId)
    {
      var services = await GetAllServicesAsync();
      if (!services.TryGetValue(serviceId, out var service))
        return null;

      return new ServiceDetailsViewModel
      {
        Id = serviceId,
        Service = service,
        GradientClasses = GetGradientClasses(serviceId),
        IconColor = GetIconColor(serviceId),
        HoverBorderColor = GetHoverBorderColor(serviceId),
        ImageUrl = GetServiceImageUrl(serviceId)
      };
    }

    // Obtém cards para o catálogo
    public async Task<List<ServiceCardViewModel>> GetServiceCardsAsync()
    {
      var services = await GetAllServicesAsync();

      return services.Select(kvp => new ServiceCardViewModel
      {
        Id = kvp.Key,
        Title = kvp.Value.Title,
        Description = kvp.Value.Description,
        ImageUrl = GetServiceImageUrl(kvp.Key),
        Price = GetServicePrice(kvp.Value),
        GlowColor = GetGlowColor(kvp.Key)
      }).ToList();
    }

    // Obtém apenas os serviços em destaque
    public async Task<List<ServiceCardViewModel>> GetFeaturedServicesAsync()
    {
      var allServices = await GetServiceCardsAsync();
      return allServices.Where(s => s.Id is "gold" or "dungeons" or "raids").ToList();
    }

    // Métodos auxiliares 
    private string GetServiceImageUrl(string serviceId)
    {
      return serviceId switch
      {
        "gold" => "images/gold-services.png",
        "dungeons" => "images/mythic-plus.png",
        "raids" => "images/raid-boosting.png",
        "pvp" => "images/pvp-services.png",
        "itens" => "images/special-itens.png",
        _ => "images/default-service.png"
      };
    }

    private string GetServicePrice(Service service)
    {
      return service.Packages.FirstOrDefault(p => p.IsPopular)?.Price ??
             service.Packages.FirstOrDefault()?.Price ??
             "Consultar";
    }

    private string GetGradientClasses(string serviceId)
    {
      return serviceId switch
      {
        "gold" => "from-yellow-400 to-yellow-600",
        "dungeons" => "from-blue-600 to-indigo-800",
        "raids" => "from-purple-800 to-red-700",
        "pvp" => "from-red-700 to-red-900",
        "itens" => "from-green-700 to-teal-600",
        _ => "from-gray-700 to-gray-900"
      };
    }

    private string GetIconColor(string serviceId)
    {
      return serviceId switch
      {
        "gold" => "text-yellow-500",
        "dungeons" => "text-blue-500",
        "raids" => "text-purple-600",
        "pvp" => "text-red-600",
        "itens" => "text-green-600",
        _ => "text-gray-500"
      };
    }

    private string GetHoverBorderColor(string serviceId)
    {
      return serviceId switch
      {
        "gold" => "border-yellow-500",
        "dungeons" => "border-blue-500",
        "raids" => "border-purple-500",
        "pvp" => "border-red-500",
        "itens" => "border-green-500",
        _ => "border-gray-500"
      };
    }

    private string GetGlowColor(string serviceId)
    {
      return serviceId switch
      {
        "gold" => "#f1c533",
        "dungeons" => "red",
        "raids" => "purple",
        "pvp" => "orangered",
        "itens" => "green",
        _ => "#808080"
      };
    }
  }
}