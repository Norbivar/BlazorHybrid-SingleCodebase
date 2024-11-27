using MyBlazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazor.Authentication
{
	public interface IAuthenticationService
	{
		public event Action<ClaimsPrincipal>? UserChanged;

		public Task<bool> ReloginAsync();
		public Task<bool> LoginAsync(string? token);
		public void LogoutAsync();
		public void RefreshAuthTokenAsync();
	}
}
