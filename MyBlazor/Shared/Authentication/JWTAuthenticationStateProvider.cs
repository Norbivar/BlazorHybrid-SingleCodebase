using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MyBlazor.Shared.Authentication
{
	public class JWTAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorage;
		private readonly HttpClient _httpClient;

		public JWTAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
		{
			_localStorage = localStorage;
			_httpClient = httpClient;
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

				var response = await _httpClient.GetAsync("account/userinfo");
				if (response.IsSuccessStatusCode)
				{
					var user = await response.Content.ReadFromJsonAsync<Shared.Models.UserModel>();
					if (user is not null)
					{
						var claims = new[]
						{
							new Claim(ClaimTypes.Name, user.Email),
							new Claim(ClaimTypes.Email, user.Email)
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
			}
			catch
			{
				Console.WriteLine("Error occured in GetAuthenticationStateAsync");
			}

			// If any error occurs, return an anonymous identity
			_httpClient.DefaultRequestHeaders.Authorization = null;
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		public async Task<bool> MarkUserAsAuthenticated(string token)
		{
			await _localStorage.SetItemAsync("jwtToken", token);
			var authState = GetAuthenticationStateAsync();
			NotifyAuthenticationStateChanged(authState);

			var result = await authState;
			return (result.User.Identity is not null && result.User.Identity.IsAuthenticated);
		}

		public async Task MarkUserAsLoggedOut()
		{
			await _localStorage.RemoveItemAsync("jwtToken");
			_httpClient.DefaultRequestHeaders.Authorization = null;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}
	}
}
