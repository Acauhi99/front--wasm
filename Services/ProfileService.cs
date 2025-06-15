using System.Net.Http.Json;
using api__dapper.domain.models;

namespace front__wasm.Services
{
  public class ProfileService
  {
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ProfileService(HttpClient httpClient, AuthService authService)
    {
      _httpClient = httpClient;
      _authService = authService;
    }

    // Get current user details
    public async Task<User?> GetCurrentUserAsync()
    {
      if (_authService.CurrentUser == null)
        return null;

      try
      {
        var user = await GetUserByIdAsync(_authService.CurrentUser.Id);
        return user;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error getting current user: {ex.Message}");
        return null;
      }
    }

    // Get user by ID
    public async Task<User?> GetUserByIdAsync(string userId)
    {
      if (string.IsNullOrEmpty(userId))
        return null;

      try
      {
        Console.WriteLine($"Fetching user with ID: {userId}");

        // Ensure the token is in the headers
        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization") &&
            _authService.CurrentUser != null)
        {
          var token = await _authService.GetTokenAsync();
          if (!string.IsNullOrEmpty(token))
          {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
          }
        }

        var user = await _httpClient.GetFromJsonAsync<User>($"api/users/{userId}");
        Console.WriteLine($"User retrieved: {user?.Name} ({user?.Email})");
        return user;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error fetching user: {ex.Message}");
        return null;
      }
    }

    // Get all users (Admin only)
    public async Task<List<User>> GetAllUsersAsync()
    {
      try
      {
        return await _httpClient.GetFromJsonAsync<List<User>>("api/users") ?? new List<User>();
      }
      catch
      {
        return new List<User>();
      }
    }

    // Update user
    public async Task<bool> UpdateUserAsync(UserUpdateModel model)
    {
      try
      {
        Console.WriteLine($"Updating user with ID: {model.Id}");
        var response = await _httpClient.PutAsJsonAsync($"api/users/{model.Id}", model);

        if (!response.IsSuccessStatusCode)
        {
          var errorContent = await response.Content.ReadAsStringAsync();
          Console.WriteLine($"Update failed: {response.StatusCode}, {errorContent}");
        }

        return response.IsSuccessStatusCode;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error updating user: {ex.Message}");
        return false;
      }
    }

    // Delete user (Admin only)
    public async Task<bool> DeleteUserAsync(string userId)
    {
      try
      {
        var response = await _httpClient.DeleteAsync($"api/users/{userId}");
        return response.IsSuccessStatusCode;
      }
      catch
      {
        return false;
      }
    }
  }

  public class UserUpdateModel
  {
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
  }
}