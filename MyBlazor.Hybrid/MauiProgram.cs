using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyBlazor.Hybrid.Services;
using MyBlazor.Shared.Authentication;
using MyBlazor.Shared.Notifications;
using System.Net.Http.Headers;

namespace MyBlazor.Hybrid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
            // If using localhost
            builder.Services.AddDevHttpClient(7030);
#else
            // If using a published development API instead of localhost
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("Production API") });
#endif
			builder.Services.AddBlazoredLocalStorage();
			builder.Services.AddScoped(async sp =>
			{
				var client = sp.GetRequiredService<HttpClient>();
				var localStorage = sp.GetRequiredService<ILocalStorageService>();

				var result = await localStorage.GetItemAsync<string>("jwtToken");
				if (result != null && result.Count() > 0)
				{
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result);
				}

				return client;
			});

            builder.Services.AddCascadingAuthenticationState();
			builder.Services.AddAuthorizationCore();

            builder.Services.AddScoped<IFetchDataService, FetchDataService>();
			builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddScoped<AuthenticationStateProvider, JWTAuthenticationStateProvider>();

			return builder.Build();
        }
    }
}