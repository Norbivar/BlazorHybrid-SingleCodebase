﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyBlazor.Hybrid.Services;
using MyBlazor.Authentication;
using MyBlazor.HTTP;
using MyBlazor.Notifications;
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

            builder.Services.AddCascadingAuthenticationState();
			builder.Services.AddAuthorizationCore();

			builder.Services.AddSingleton<INotificationService, NotificationService>()
				.AddScoped<IFetchDataService, FetchDataService>()
				.AddScoped<HttpHubService>();

			builder.Services.AddScoped<IAuthenticationService, ClientAuthenticationService>();
			builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

			return builder.Build();
        }
    }
}