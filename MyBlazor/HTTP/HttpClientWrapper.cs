using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.Net.Http.Headers;
using System.Net.Http;

namespace MyBlazor.HTTP
{
	public class HttpClientWrapper
	{
		protected HttpClient _httpClient;
		public HttpClientWrapper(HttpClient httpClient) => _httpClient = httpClient;

		protected async Task<HTTPRes<T1>> PostRequestAsync<T1, T2>(
			string url,
			T2 input,
			JsonTypeInfo jsonTypeInput,
			JsonTypeInfo<T1> jsonTypeOutput,
			Action<HttpRequestMessage>? requestModification = null)
		{
			using (var request = new HttpRequestMessage())
			{
				request.Method = HttpMethod.Post;
				request.RequestUri = new Uri(url, UriKind.Relative);
				request.Headers.Add("User-Agent", "Blazor-Client");
				request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				if (requestModification is not null)
				{
					requestModification(request);
				}

				request.Content = new StringContent(JsonSerializer.Serialize(input, jsonTypeInput), Encoding.UTF8, "application/json");

				HttpResponseMessage response = await _httpClient.SendAsync(request);
				try
				{
					var response_body = JsonSerializer.Deserialize<T1>(
						await response.Content.ReadAsStringAsync(),
						jsonTypeOutput
					);

					return new HTTPRes<T1>
					{
						Success = (response.IsSuccessStatusCode && response_body is not null),
						Value = response_body
					};
				}
				catch (Exception)
				{
				}

				return new HTTPRes<T1>
				{
					Success = false,
					Value = default(T1)
				};
			}
		}

		protected async Task<bool> PostRequestAsync<T2>(
			string url,
			T2 input,
			JsonTypeInfo jsonTypeInput,
			Action<HttpRequestMessage>? requestModification = null)
		{
			using (var request = new HttpRequestMessage())
			{
				request.Method = HttpMethod.Post;
				request.RequestUri = new Uri(url, UriKind.Relative);
				request.Headers.Add("User-Agent", "Blazor-Client");
				request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				if (requestModification is not null)
				{
					requestModification(request);
				}

				request.Content = new StringContent(JsonSerializer.Serialize(input, jsonTypeInput), Encoding.UTF8, "application/json");

				HttpResponseMessage response = await _httpClient.SendAsync(request);
				try
				{
					return response.IsSuccessStatusCode;
				}
				catch (Exception)
				{
				}

				return false;
			}
		}

		protected async Task<HTTPRes<T1>> GetRequestAsync<T1>(
			string url,
			JsonTypeInfo<T1> jsonTypeOutput,
			Action<HttpRequestMessage>? requestModification = null)
		{
			using (var request = new HttpRequestMessage())
			{
				request.Method = HttpMethod.Get;
				request.RequestUri = new Uri(url, UriKind.Relative);
				request.Headers.Add("User-Agent", "Blazor-Client");
				request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				if (requestModification is not null)
				{
					requestModification(request);
				}

				HttpResponseMessage response = await _httpClient.SendAsync(request);
				try
				{
					var response_body = JsonSerializer.Deserialize<T1>(
						await response.Content.ReadAsStringAsync(),
						jsonTypeOutput
					);

					return new HTTPRes<T1>
					{
						Success = (response.IsSuccessStatusCode && response_body is not null),
						Value = response_body
					};
				}
				catch (Exception)
				{
				}

				return new HTTPRes<T1>
				{
					Success = false,
					Value = default(T1)
				};
			}
		}
	}
	public class HTTPRes<T>
	{
		public bool Success = false;
		public T? Value { get; set; }
	}
}
