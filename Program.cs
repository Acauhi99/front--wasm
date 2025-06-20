using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using front__wasm;
using front__wasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Original HttpClient for local resources
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add API client for backend calls
builder.Services.AddHttpClient("ApiClient", client =>
{
  client.BaseAddress = new Uri("http://localhost:5090/");
});

// Register services
builder.Services.AddScoped<CartService>();

// Register AuthService 
builder.Services.AddScoped(sp =>
{
  var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
  var httpClient = httpClientFactory.CreateClient("ApiClient");
  var jsRuntime = sp.GetRequiredService<IJSRuntime>();
  return new AuthService(httpClient, jsRuntime);
});

// Register ProfileService
builder.Services.AddScoped(sp =>
{
  var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
  var httpClient = httpClientFactory.CreateClient("ApiClient");
  var authService = sp.GetRequiredService<AuthService>();
  return new ProfileService(httpClient, authService);
});

// Register ServiceManager 
builder.Services.AddScoped(sp =>
{
  var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
  var httpClient = httpClientFactory.CreateClient("ApiClient");
  var logger = sp.GetService<ILogger<ServiceManager>>();
  return new ServiceManager(httpClient, logger);
});

// Register DashboardService
builder.Services.AddScoped(sp =>
{
  var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
  var httpClient = httpClientFactory.CreateClient("ApiClient");
  var authService = sp.GetRequiredService<AuthService>();
  var logger = sp.GetService<ILogger<DashboardService>>();
  return new DashboardService(httpClient, authService, logger);
});

builder.Logging.SetMinimumLevel(LogLevel.Debug);

await builder.Build().RunAsync();