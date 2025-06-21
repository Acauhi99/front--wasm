using System.Net.Http.Json;
using front__wasm.Models;

namespace front__wasm.Services
{
  public class CheckoutService
  {
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly CartService _cartService;
    private readonly ILogger<CheckoutService>? _logger;

    public CheckoutService(
        HttpClient httpClient,
        AuthService authService,
        CartService cartService,
        ILogger<CheckoutService>? logger = null)
    {
      _httpClient = httpClient;
      _authService = authService;
      _cartService = cartService;
      _logger = logger;
    }

    public async Task<SellCreateResponse?> CreateSellAsync(string paymentMethod)
    {
      try
      {
        _logger?.LogInformation($"Creating new sell with payment method: {paymentMethod}");

        var token = await _authService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Converter itens do carrinho para o modelo de requisição
        var sellRequest = new SellRequest
        {
          Items = _cartService.CartItems.Select(i => new SellItemRequest
          {
            Title = i.Title,
            Description = i.Description,
            UnitPrice = CartService.ParseCurrency(i.UnitPrice),
            Quantity = i.Quantity
          }).ToList(),
          Subtotal = _cartService.GetSubtotal(),
          ServiceFee = 2.00m,
          Total = _cartService.GetSubtotal() + 2.00m,
          PaymentMethod = paymentMethod
        };

        var response = await _httpClient.PostAsJsonAsync("/api/sells", sellRequest);

        if (response.IsSuccessStatusCode)
        {
          var createResponse = await response.Content.ReadFromJsonAsync<SellCreateResponse>();
          _logger?.LogInformation($"Successfully created sell with ID: {createResponse?.Id}");
          return createResponse;
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        _logger?.LogWarning($"Error creating sell: {response.StatusCode}, {errorContent}");

        return null;
      }
      catch (Exception ex)
      {
        _logger?.LogError(ex, "Exception creating sell");
        return null;
      }
    }
  }
}