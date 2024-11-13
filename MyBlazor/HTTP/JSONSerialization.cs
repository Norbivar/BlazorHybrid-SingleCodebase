using MyBlazor.Models;
using System.Text.Json.Serialization;

namespace MyBlazor.HTTP
{

	[JsonSerializable(typeof(LoginRegisterModel))]
	[JsonSerializable(typeof(LoginResultModel))]
	[JsonSerializable(typeof(RegisterResultModel))]
	[JsonSerializable(typeof(UserModel))]
	[JsonSerializable(typeof(string))]
	public partial class AuthenticationJsonContext : JsonSerializerContext
	{
	}
}
