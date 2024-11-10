using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlazor.SharedUI.Pages.Models
{
	public class LoginRegisterModel
	{
		[Required(ErrorMessage = "Username is required")]
		[MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[MinLength(3, ErrorMessage = "Password must be at least 3 characters")]
		public string Password { get; set; }

		public LoginRegisterModel ShallowCopy()
		{
			return (LoginRegisterModel)MemberwiseClone();
		}
	}

	public class RegisterResultModel
	{
		public IEnumerable<string> Errors { get; set; }
	}

	public class LoginResultModel
	{
		public string JWTToken { get; set; }
	}
}
