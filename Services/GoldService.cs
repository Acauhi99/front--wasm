using System.Net.Http.Json;
using front__wasm.Models;


namespace front__wasm.Services
{
  public class GoldService
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<GoldService>? _logger;

    public GoldService(HttpClient httpClient, ILogger<GoldService>? logger = null)
    {
      _httpClient = httpClient;
      _logger = logger;
    }

    public async Task<IEnumerable<GoldPackage>> GetGoldPackagesAsync()
    {
      try
      {
        var packages = await _httpClient.GetFromJsonAsync<IEnumerable<GoldPackage>>("data/gold.json");

        _logger?.LogInformation($"Loaded {packages?.Count() ?? 0} gold packages");

        return packages ?? new List<GoldPackage>();
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Erro ao carregar pacotes de ouro: {Message}", ex.Message);
        Console.WriteLine($"Error loading gold packages: {ex.Message}");
        return new List<GoldPackage>();
      }
    }
  }
}
