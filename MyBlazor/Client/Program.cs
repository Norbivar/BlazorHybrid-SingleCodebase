using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyBlazor.Client;
using MyBlazor.Client.Services;
using MyBlazor.Shared.Authentication;
using MyBlazor.Shared.HTTP;
using MyBlazor.Shared.Notifications;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

//builder.Services.AddOidcAuthentication(options =>
//{
//	builder.Configuration.Bind("Oidc", options.ProviderOptions);
//});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped(async sp =>
{
	// TODO: I don't know if this makes sense or does anything, but...
	var client = sp.GetRequiredService<HttpClient>();
	var localStorage = sp.GetRequiredService<ILocalStorageService>();

	var result = await localStorage.GetItemAsync<string>("jwtToken");
	if (result != null && result.Count() > 0)
	{
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result);
	}

	return client;
});

builder.Services.AddScoped<IFetchDataService, FetchDataService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddScoped<AuthenticationStateProvider, JWTAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthenticationStateProvider, JWTAuthenticationStateProvider>();
builder.Services.AddScoped<HttpHubService>();

await builder.Build().RunAsync();
