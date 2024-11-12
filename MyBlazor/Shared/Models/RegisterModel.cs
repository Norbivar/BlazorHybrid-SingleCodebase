using System.ComponentModel.DataAnnotations;

namespace MyBlazor.Shared.Pages.Models
{
	public class LoginRegisterModel
	{
		[Required(ErrorMessage = "Username is required")]
		[MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
		public required string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[MinLength(3, ErrorMessage = "Password must be at least 3 characters")]
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
