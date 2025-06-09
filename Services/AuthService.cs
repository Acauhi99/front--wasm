using System.Net.Http.Json;
using System.Text.Json;
using api__dapper.domain.models;
using Microsoft.JSInterop;

namespace front__wasm.Services
{
  public class AuthService
  {
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    public User? CurrentUser { get; private set; }
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
      var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "user");

      if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
      {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        CurrentUser = JsonSerializer.Deserialize<User>(userJson);
        NotifyStateChanged();
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

          if (authResult != null)
          {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", authResult.Token);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user", JsonSerializer.Serialize(authResult.User));

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.Token);
            CurrentUser = authResult.User;
            NotifyStateChanged();
            return true;
          }
        }

        return false;
      }
      catch
      {
        return false;
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