
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MyBlazor.Authentication
{
	public class CustomAuthenticationStateProvider : AuthenticationStateProvider
	{
		private AuthenticationState authenticationState;

		public CustomAuthenticationStateProvider(HttpClient httpClient, IAuthenticationService service)
		{
			authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

			service.UserChanged += (newUser) =>
			{
				authenticationState = new AuthenticationState(newUser);
				NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
			};
		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(authenticationState);
	}
}