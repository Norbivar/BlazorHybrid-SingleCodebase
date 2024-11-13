﻿using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazor.Authentication
{
	public class IAuthenticationStateProvider : AuthenticationStateProvider
	{
		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			throw new NotImplementedException();
		}

		public virtual async Task<bool> TryMarkUserAsAuthenticated(string token)
		{
			throw new NotImplementedException();
		}
	}
}
