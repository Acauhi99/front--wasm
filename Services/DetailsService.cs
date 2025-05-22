using System.Net.Http.Json;
using front__wasm.Models;

namespace front__wasm.Services
{
  public class DetailsService
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<DetailsService>? _logger;
    private Dictionary<string, ServiceDetails>? _cachedServices;

    public DetailsService(HttpClient httpClient, ILogger<DetailsService>? logger = null)
    {
      _httpClient = httpClient;
      _logger = logger;
    }

    public async Task<ServiceDetails?> GetServiceDetailsAsync(string serviceType)
    {
      try
      {
        // Load all services if not cached
        if (_cachedServices == null)
        {
          var response = await _httpClient.GetFromJsonAsync<Dictionary<string, ServiceDetails>>("data/services-details.json");
          _cachedServices = response;
        }

        // Return the requested service type
        if (_cachedServices != null && _cachedServices.TryGetValue(serviceType, out var details))
        {
          _logger?.LogInformation("Loaded {ServiceType} service details successfully.", serviceType);
          return details;
        }

        _logger?.LogWarning("Service type {ServiceType} not found in services data.", serviceType);
        return null;
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error loading service details for {ServiceType}: {Message}", serviceType, ex.Message);
        Console.WriteLine($"Error loading service details for {serviceType}: {ex.Message}");
        return null;
      }
    }
  }
}