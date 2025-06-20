using System.Net.Http.Json;
using front__wasm.Models;

namespace front__wasm.Services
{
  public class DashboardService
  {
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly ILogger<DashboardService>? _logger;

    public DashboardService(HttpClient httpClient, AuthService authService, ILogger<DashboardService>? logger = null)
    {
      _httpClient = httpClient;
      _authService = authService;
      _logger = logger;
    }

    // Get all sells (Admin only)
    public async Task<List<SellResponse>> GetAllSellsAsync()
    {
      try
      {
        _logger?.LogInformation("Fetching all sells for admin dashboard");

        await EnsureAuthHeaderAsync();
        var response = await _httpClient.GetAsync("api/sells");

        if (response.IsSuccessStatusCode)
        {
          var sells = await response.Content.ReadFromJsonAsync<List<SellResponse>>();
          _logger?.LogInformation($"Successfully fetched {sells?.Count ?? 0} sells");
          return sells ?? new List<SellResponse>();
        }
        else
        {
          var error = await response.Content.ReadAsStringAsync();
          _logger?.LogWarning($"Failed to get sells: {response.StatusCode}, {error}");
          return new List<SellResponse>();
        }
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Error fetching all sells");
        return new List<SellResponse>();
      }
    }

    // Get sell by ID (Admin only)
    public async Task<SellResponse?> GetSellByIdAsync(string sellId)
    {
      try
      {
        _logger?.LogInformation($"Fetching sell with ID: {sellId}");

        await EnsureAuthHeaderAsync();
        var response = await _httpClient.GetAsync($"api/sells/{sellId}");

        if (response.IsSuccessStatusCode)
        {
          var sell = await response.Content.ReadFromJsonAsync<SellResponse>();
          _logger?.LogInformation($"Successfully fetched sell {sellId}");
          return sell;
        }
        else
        {
          var error = await response.Content.ReadAsStringAsync();
          _logger?.LogWarning($"Failed to get sell {sellId}: {response.StatusCode}, {error}");
          return null;
        }
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, $"Error fetching sell {sellId}");
        return null;
      }
    }

    // Helper method to ensure auth header is set
    private async Task EnsureAuthHeaderAsync()
    {
      var token = await _authService.GetTokenAsync();

      if (!string.IsNullOrEmpty(token))
      {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
      }
      else
      {
        throw new UnauthorizedAccessException("User is not authenticated");
      }
    }
  }
}