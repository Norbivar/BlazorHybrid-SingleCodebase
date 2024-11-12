using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyBlazor.Shared.HTTP;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MyBlazor.Shared.Authentication
{
	public class JWTAuthenticationStateProvider : IAuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorage;
		private readonly HttpClient _httpClient;
		private readonly HttpHubService _httpHub;

		public JWTAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient, HttpHubService httpHub)
		{
			_localStorage = localStorage;
			_httpClient = httpClient;
			_httpHub = httpHub;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await _localStorage.GetItemAsync<string>("jwtToken");
			if (string.IsNullOrEmpty(token))
			{
				return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())); // anonymous identity
			}

			try
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpHub.Get.UserInfo();
				if (response.Success && response.Value is not null)
				{
					var claims = new[]
					{
						new Claim(ClaimTypes.Name, response.Value.Email),
						new Claim(ClaimTypes.Email, response.Value.Email)
					};

					var identity = new ClaimsIdentity(claims, "jwtAuth");
					var userPrincipal = new ClaimsPrincipal(identity);

					return new AuthenticationState(userPrincipal);
				}
				else
				{
					Console.WriteLine("Clientside authentication successful but did not receive model!");
				}
			}
			catch
			{
				Console.WriteLine("Error occured in GetAuthenticationStateAsync");
			}

			// If any error occurs, return an anonymous identity
			_httpClient.DefaultRequestHeaders.Authorization = null;
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		public new async Task<bool> TryMarkUserAsAuthenticated(string token)
		{
			await _localStorage.SetItemAsync("jwtToken", token);
			var authState = GetAuthenticationStateAsync();
			NotifyAuthenticationStateChanged(authState);

			var result = await authState;
			return (result.User.Identity is not null && result.User.Identity.IsAuthenticated);
		}

		public async Task TryMarkUserAsLoggedOut()
		{
			await _localStorage.RemoveItemAsync("jwtToken");
			_httpClient.DefaultRequestHeaders.Authorization = null;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}
	}
}
