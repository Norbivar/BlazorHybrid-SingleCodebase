using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyBlazor.HTTP;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MyBlazor.Authentication
{
	public class JWTAuthenticationStateProvider : IAuthenticationStateProvider
	{
		private readonly string AuthenticationTokenType = "jwtToken";

		private readonly ILocalStorageService _localStorage;
		private readonly HttpClient _httpClient;
		private readonly HttpHubService _httpHub;
		static private (string token, AuthenticationState state) activeAuthenticationState;

		public JWTAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient, HttpHubService httpHub)
		{
			_localStorage = localStorage;
			_httpClient = httpClient;
			_httpHub = httpHub;
			AuthenticationStateChanged += OnAuthenticationStateChanged;
		}

		private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
		{
			await task;
		}

		// This is called when logging in, but also every time 
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await _localStorage.GetItemAsync<string>(AuthenticationTokenType);
			if (!string.IsNullOrEmpty(token))
			{
				if (token == activeAuthenticationState.token && activeAuthenticationState.state is not null)
				{
					return activeAuthenticationState.state; // cached path
				}
				else
				{
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

							activeAuthenticationState = (token, new AuthenticationState(userPrincipal));
							return activeAuthenticationState.state;
						}
						else
						{
							Console.WriteLine("Failed to validate JWT token!");
						}
					}
					catch
					{
						Console.WriteLine("Error occured in GetAuthenticationStateAsync");
					}
				}
			}

			// If any error occurs, return an anonymous identity
			_httpClient.DefaultRequestHeaders.Authorization = null;
			await _localStorage.RemoveItemAsync(AuthenticationTokenType); // Login was not successful, so delete the token to clean up

			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		public override async Task<Result> TryLoginAsync()
		{
			var token = await _localStorage.GetItemAsync<string>(AuthenticationTokenType);
			if (string.IsNullOrEmpty(token))
			{
				Result.Failure(Error.NotFound("Token", "Token not found in local storage!"));
			}

			var authState = GetAuthenticationStateAsync();
			var result = await authState;

			if (result.User != null && result.User.Identity != null && result.User.Identity.IsAuthenticated)
			{
				NotifyAuthenticationStateChanged(authState);
				return Result.Success();
			}

			return Result.Failure(Error.Failure("Invalid", "Token was not valid, user is not authenticated."));
		}

		public override async Task<Result> TryLoginWithTokenAsync(string token)
		{
			await _localStorage.SetItemAsync(AuthenticationTokenType, token);
			return await TryLoginAsync();
		}

		//public async Task TryMarkUserAsLoggedOut()
		//{
		//	await _localStorage.RemoveItemAsync("jwtToken");
		//	_httpClient.DefaultRequestHeaders.Authorization = null;
		//	NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		//}
	}
}
