using MyBlazor.Shared.Pages.Models;
using System.Text.Json;
using System.Text;
using MyBlazor.Shared.Models;
using MyBlazor.Shared.HTTP;
using static MyBlazor.Shared.HTTP.HttpHubService;

namespace MyBlazor.Shared.HTTP
{
	public class HttpHubService
	{
		#region Helpers
		public class HttpRoutesBase
		{
			HttpClient _httpClient;
			public HttpRoutesBase(HttpClient httpClient) => _httpClient = httpClient;

			#region Helpers
			protected async Task<HTTPRes<T1>> DoPostRequestAsync<T1, T2>(string url, T2 inputObj)
			{
				var content = new StringContent(JsonSerializer.Serialize(inputObj), Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync(url, content)
					.ConfigureAwait(false);

				bool success = false;
				var valueToReturn = default(T1);
				try
				{
					var response_body = JsonSerializer.Deserialize<T1>(
						await response.Content.ReadAsStringAsync()
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

			protected async Task<HTTPRes<T1>> DoGetRequestAsync<T1>(string url)
			{
				var response = await _httpClient.GetAsync(url)
					.ConfigureAwait(false);

				bool success = false;
				var valueToReturn = default(T1);
				try
				{
					var response_body = JsonSerializer.Deserialize<T1>(
						await response.Content.ReadAsStringAsync()
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

		public class HttpPostRoutes : HttpRoutesBase
		{
			public HttpPostRoutes(HttpClient httpClient) : base(httpClient) { }
			public async Task<HTTPRes<LoginResultModel>> Login(LoginRegisterModel data) => await DoPostRequestAsync<LoginResultModel, LoginRegisterModel>("account/login", data);
			public async Task<HTTPRes<RegisterResultModel>> Register(LoginRegisterModel data) => await DoPostRequestAsync<RegisterResultModel, LoginRegisterModel>("account/register", data);

		}
		public class HttpGetRoutes : HttpRoutesBase
		{
			public HttpGetRoutes(HttpClient httpClient) : base(httpClient) { }
			public async Task<HTTPRes<UserModel>> UserInfo() => await DoGetRequestAsync<UserModel>("account/userinfo");
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
}
