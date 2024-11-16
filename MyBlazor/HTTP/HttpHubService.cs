﻿using MyBlazor.Models;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization.Metadata;

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

		public class HttpPostRoutes : HttpRoutesBase
		{
			public HttpPostRoutes(HttpClient httpClient) : base(httpClient) { }
			public async Task<HTTPRes<LoginResultModel>> Login(LoginRegisterModel data)
			{
				return await DoPostRequestAsync(
					"account/login",
					data,
					AuthenticationJsonContext.Default.LoginRegisterModel,
					AuthenticationJsonContext.Default.LoginResultModel
				);
			}

			public async Task<HTTPRes<RegisterResultModel>> Register(LoginRegisterModel data)
			{
				return await DoPostRequestAsync(
					"account/register",
					data,
					AuthenticationJsonContext.Default.LoginRegisterModel,
					AuthenticationJsonContext.Default.RegisterResultModel
				);
			}
		}
		public class HttpGetRoutes : HttpRoutesBase
		{
			public HttpGetRoutes(HttpClient httpClient) : base(httpClient) { }
			public async Task<HTTPRes<UserModel>> UserInfo()
			{
				return await DoGetRequestAsync(
					"account/userinfo",
					AuthenticationJsonContext.Default.UserModel
				);
			}
		}

		private readonly HttpClient _Http;
		public readonly HttpPostRoutes Post;
		public readonly HttpGetRoutes Get;


		public HttpHubService(HttpClient http)
		{
			this._Http = http;
			Post = new HttpPostRoutes(this._Http);
			Get = new HttpGetRoutes(this._Http);
		}
	}

	#region Helpers
	public class HttpRoutesBase
	{
		protected HttpClient _httpClient;
		public HttpRoutesBase(HttpClient httpClient) => _httpClient = httpClient;

		#region Helpers
		protected async Task<HTTPRes<T1>> DoPostRequestAsync<T1, T2>(string url, T2 inputObj, JsonTypeInfo jsonTypeInput, JsonTypeInfo<T1> jsonTypeOutput)
		{
			var content = new StringContent(JsonSerializer.Serialize(inputObj, jsonTypeInput), Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(url, content)
				.ConfigureAwait(false);

			bool success = false;
			var valueToReturn = default(T1);
			try
			{
				var response_body = JsonSerializer.Deserialize<T1>(
					await response.Content.ReadAsStringAsync(),
					jsonTypeOutput
				);

				success = response.IsSuccessStatusCode;
				if (response_body != null)
					valueToReturn = response_body;
			}
			catch (Exception)
			{

			}

			return new HTTPRes<T1>
			{
				Success = success,
				Value = valueToReturn
			};
		}

		protected async Task<HTTPRes<T1>> DoGetRequestAsync<T1>(string url, JsonTypeInfo<T1> jsonTypeOutput)
		{
			var response = await _httpClient.GetAsync(url)
				.ConfigureAwait(false);

			bool success = false;
			var valueToReturn = default(T1);
			try
			{
				var response_body = JsonSerializer.Deserialize<T1>(
					await response.Content.ReadAsStringAsync(),
					jsonTypeOutput
				);

				success = response.IsSuccessStatusCode;
				if (response_body != null)
					valueToReturn = response_body;
			}
			catch (Exception)
			{

			}

			return new HTTPRes<T1>
			{
				Success = success,
				Value = valueToReturn
			};
			#endregion

		}
	}
	public class HTTPRes<T>
	{
		public bool Success = false;
		public T? Value { get; set; }
	}

	#endregion
}
