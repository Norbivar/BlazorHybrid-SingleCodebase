using MyBlazor.Models;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Net.Http.Headers;
using System;

namespace MyBlazor.HTTP
{
	public class HttpHubService
	{
		// How to create a new HTTP path handler:
		// 1.: Create input-output models
		// 2.: Add those models to a JsonContext (or create a new one for them)
		// 3.: Create "shortcut" function in either HTTP method group below
		// 4.: Set parameters accordingly
		// 5.: Enjoy!

		public class HttpPostRoutes : HttpClientWrapper
		{
			public HttpPostRoutes(HttpClient httpClient) : base(httpClient) { }
			public async Task<HTTPRes<LoginResultModel>> Login(LoginRegisterModel data)
			{
				return await PostRequestAsync(
					url: "account/login",
					input: data,
					jsonTypeInput: AuthenticationJsonContext.Default.LoginRegisterModel,
					jsonTypeOutput: AuthenticationJsonContext.Default.LoginResultModel
				);
			}

			public async Task<HTTPRes<RegisterResultModel>> Register(LoginRegisterModel data)
			{
				return await PostRequestAsync(
					url: "account/register",
					input: data,
					jsonTypeInput: AuthenticationJsonContext.Default.LoginRegisterModel,
					jsonTypeOutput: AuthenticationJsonContext.Default.RegisterResultModel
				);
			}
			public async Task<bool> ValidateAuthToken(string usedToken)
			{
				return await PostRequestAsync(
					url: "account/validate_token",
					input: new EmptyModel(),
					jsonTypeInput: AuthenticationJsonContext.Default.EmptyModel,
					requestModification: message => message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usedToken)
				);
			}

			public async Task<HTTPRes<UserAuthTokenModel>> RefreshAuthToken()
			{
				return await PostRequestAsync(
					url: "account/refresh_token",
					input: new EmptyModel(),
					jsonTypeInput: AuthenticationJsonContext.Default.EmptyModel,
					jsonTypeOutput: AuthenticationJsonContext.Default.UserAuthTokenModel
				);
			}
		}
		public class HttpGetRoutes : HttpClientWrapper
		{
			public HttpGetRoutes(HttpClient httpClient) : base(httpClient) { }
		}

		private readonly HttpClient _Http;
		public readonly HttpPostRoutes Post;
		public readonly HttpGetRoutes Get;

		public HttpHubService(HttpClient http)
		{
			_Http = http;
			Post = new HttpPostRoutes(http);
			Get = new HttpGetRoutes(http);
		}
	}
}
