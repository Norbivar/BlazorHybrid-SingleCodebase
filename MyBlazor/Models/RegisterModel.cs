using System.Text.Json.Serialization;

namespace MyBlazor.Models
{
	public class EmptyModel
	{
	}

	public class LoginRegisterModel
	{
		public required string Email { get; set; }

		public required string Password { get; set; }
	}

	public class RegisterResultModel
	{
		public required IEnumerable<string> Errors { get; set; }
	}

	public class LoginResultModel
	{
		public string? JWTToken { get; set; }
	}
}
