using System.Net.Http.Json;
using front__wasm.Models;

namespace front__wasm.Services
{
  public class ServiceManager
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceManager>? _logger;
    private List<Service>? _cachedServices;

    public ServiceManager(HttpClient httpClient, ILogger<ServiceManager>? logger = null)
    {
      _httpClient = httpClient;
      _logger = logger;
    }

    // Carrega todos os serviços da API
    public async Task<List<Service>> GetAllServicesAsync()
    {
      if (_cachedServices != null)
        return _cachedServices;

      try
      {
        _logger?.LogInformation("Fetching services from API");
        var services = await _httpClient.GetFromJsonAsync<List<Service>>("api/services");

        if (services == null)
        {
          _logger?.LogWarning("API call successful, but services came back null");
          return new List<Service>();
        }

        _logger?.LogInformation($"Successfully loaded {services.Count} services from API");
        _cachedServices = services;
        return _cachedServices;
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error loading services from API: {Message}", ex.Message);
        return new List<Service>();
      }
    }

    // Obtém um serviço específico pelo key
    public async Task<ServiceDetailsViewModel?> GetServiceDetailsAsync(string serviceKey)
    {
      try
      {
        _logger?.LogInformation($"Fetching service details for key: {serviceKey}");
        var service = await _httpClient.GetFromJsonAsync<Service>($"api/services/key/{serviceKey}");

        if (service == null)
        {
          _logger?.LogWarning($"Service with key {serviceKey} not found");
          return null;
        }

        return new ServiceDetailsViewModel
        {
          Id = service.Key,
          Service = service,
          GradientClasses = GetGradientClasses(service.Key),
          IconColor = GetIconColor(service.Key),
          HoverBorderColor = GetHoverBorderColor(service.Key),
          ImageUrl = GetServiceImageUrl(service.Key)
        };
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error fetching service details: {Message}", ex.Message);

        var services = await GetAllServicesAsync();
        var service = services.FirstOrDefault(s => s.Key == serviceKey);

        if (service == null)
          return null;

        return new ServiceDetailsViewModel
        {
          Id = service.Key,
          Service = service,
          GradientClasses = GetGradientClasses(service.Key),
          IconColor = GetIconColor(service.Key),
          HoverBorderColor = GetHoverBorderColor(service.Key),
          ImageUrl = GetServiceImageUrl(service.Key)
        };
      }
    }

    // Obtém cards para o catálogo
    public async Task<List<ServiceCardViewModel>> GetServiceCardsAsync()
    {
      var services = await GetAllServicesAsync();

      return services.Select(service => new ServiceCardViewModel
      {
        Id = service.Key,
        Title = service.Title,
        Description = service.Description,
        ImageUrl = GetServiceImageUrl(service.Key),
        Price = GetServicePrice(service),
        GlowColor = GetGlowColor(service.Key)
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