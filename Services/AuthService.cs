using System.Net.Http.Json;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using api__dapper.domain.models;
using Microsoft.JSInterop;

namespace front__wasm.Services
{
  public class AuthService
  {
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    public User? CurrentUser { get; set; }
    public bool IsAuthenticated => CurrentUser != null;
    public event Action? OnChange;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
      _httpClient = httpClient;
      _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
      var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

      if (!string.IsNullOrEmpty(token))
      {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Parse user info from token instead of localStorage
        CurrentUser = ParseUserFromToken(token);

        if (CurrentUser != null)
        {
          // Store updated user info in localStorage
          await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user", JsonSerializer.Serialize(CurrentUser));
          NotifyStateChanged();
        }
      }
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
      try
      {
        var loginRequest = new { Email = email, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

        if (response.IsSuccessStatusCode)
        {
          var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();

          if (authResult != null && !string.IsNullOrEmpty(authResult.Token))
          {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", authResult.Token);

            // Parse user from token
            CurrentUser = ParseUserFromToken(authResult.Token);

            if (CurrentUser != null)
            {
              // Store parsed user info in localStorage
              await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user", JsonSerializer.Serialize(CurrentUser));
              _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.Token);
              NotifyStateChanged();
              return true;
            }
          }
        }

        return false;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Login error: {ex.Message}");
        return false;
      }
    }

    private User? ParseUserFromToken(string token)
    {
      try
      {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        return new User
        {
          Id = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value ?? string.Empty,
          Name = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? string.Empty,
          Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty,
          Role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "User",
          CreatedAt = DateTime.Now // Not available in token, using current time as fallback
        };
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error parsing token: {ex.Message}");
        return null;
      }
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
      try
      {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", model);
        return response.IsSuccessStatusCode;
      }
      catch
      {
        return false;
      }
    }

    public async Task LogoutAsync()
    {
      await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
      await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user");
      _httpClient.DefaultRequestHeaders.Authorization = null;
      CurrentUser = null;
      NotifyStateChanged();
    }

    public async Task<string> GetTokenAsync()
    {
      try
      {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
      }
      catch
      {
        return string.Empty;
      }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
  }

  public class AuthResponse
  {
    public string Token { get; set; } = string.Empty;
    public User User { get; set; } = new User();
  }

  public class RegisterModel
  {
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
  }
}