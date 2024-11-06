using System;
using System.ComponentModel.DataAnnotations;

namespace MyBlazor.SharedUI.Pages.Models
{
	public class RegisterModel
	{
		[Required(ErrorMessage = "Username is required")]
		[MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[MinLength(3, ErrorMessage = "Password must be at least 3 characters")]
		public string Password { get; set; }

		public RegisterModel ShallowCopy()
		{
            return (RegisterModel)MemberwiseClone();
        }
	}
}
