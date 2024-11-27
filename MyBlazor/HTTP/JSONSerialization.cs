using MyBlazor.Models;
using System.Text.Json.Serialization;

namespace MyBlazor.HTTP
{

	[JsonSerializable(typeof(LoginRegisterModel))]
	[JsonSerializable(typeof(LoginResultModel))]
	[JsonSerializable(typeof(RegisterResultModel))]
	[JsonSerializable(typeof(UserAuthTokenModel))]
	[JsonSerializable(typeof(EmptyModel))]
	[JsonSerializable(typeof(string))]
	public partial class AuthenticationJsonContext : JsonSerializerContext
	{
	}
}
