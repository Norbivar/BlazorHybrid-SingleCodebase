using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MyBlazor.HTTP;
using MyBlazor.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MyBlazor.Authentication
{
	public class ClientAuthenticationService : IAuthenticationService
	{
		private readonly string AuthenticationTokenType = "jwtToken";

		private readonly ILocalStorageService _localStorage;
		private readonly HttpClient _httpClient;
		private readonly HttpHubService _httpHub;
		private readonly ILogger<ClientAuthenticationService> _logger;

		private ClaimsPrincipal? currentUser;

		public ClaimsPrincipal? CurrentUser
		{
			get { return currentUser ?? new(); }
			set
			{
				var old = currentUser;
				currentUser = value;

				if (currentUser != old && UserChanged is not null)
				{
					UserChanged(currentUser);
				}
			}
		}

		public event Action<ClaimsPrincipal>? UserChanged;

		public ClientAuthenticationService(ILogger<ClientAuthenticationService> logger, ILocalStorageService localStorage, HttpClient httpClient, HttpHubService httpHub)
		{
			_logger = logger;
			_localStorage = localStorage;
			_httpClient = httpClient;
			_httpHub = httpHub;
		}

		private async Task<bool> AuthenticateTokenAsync(string? token)
		{
			if (string.IsNullOrEmpty(token))
			{
				_logger.LogError("Token is null/empty!");
				return false;
			}

			var validateResult = await _httpHub.Post.ValidateAuthToken(token);
			if (!validateResult)
			{
				_logger.LogError("Token was not valid, user is not authenticated.");
				return false;
			}

			_logger.LogInformation("Token is valid!");
			return true;
		}

		public async Task<bool> ReloginAsync()
		{
			var storedToken = await _localStorage.GetItemAsStringAsync(AuthenticationTokenType);
			if (!string.IsNullOrEmpty(storedToken))
			{
				var relogResult = await LoginAsync(storedToken);
				if (relogResult)
				{
					return true;
				}
			}

			// Failed to re-authenticate (or missing token), delete everything just in case
			LogoutAsync();
			return false;
		}

		public async Task<bool> LoginAsync(string? token)
		{
			var authenticateRes = await AuthenticateTokenAsync(token);
			if (authenticateRes)
			{
				var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
				if (jwtToken is not null)
				{
					var _ = _localStorage.SetItemAsStringAsync(AuthenticationTokenType, token);
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

					CurrentUser = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwtAuth"));
					return true;
				}
			}

			return false;
		}

		public async void LogoutAsync()
		{
			await _localStorage.RemoveItemAsync(AuthenticationTokenType);
			_httpClient.DefaultRequestHeaders.Authorization = null;
			CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
		}

		public async void RefreshAuthTokenAsync()
		{
			using (_logger.BeginScope("Refresh Auth Token"))
			{
				var res = await _httpHub.Post.RefreshAuthToken();
				if (res.Success && res.Value is not null)
				{
					var _ = _localStorage.SetItemAsStringAsync(AuthenticationTokenType, res.Value.Token);
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", res.Value.Token);
					_logger.LogInformation("Success");
				}
				else
				{
					_logger.LogWarning("Failed");
				}
			}
		}
	}
}
