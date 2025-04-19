using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using front__wasm;
using front__wasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<GoldService>();

// Add logging configuration
builder.Logging.SetMinimumLevel(LogLevel.Debug);

await builder.Build().RunAsync();
