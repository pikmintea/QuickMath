using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuickMath.Shared.Services;
using QuickMath.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the QuickMath.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
