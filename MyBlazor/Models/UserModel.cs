using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazor.Models
{
	// The information retrieved by the client when logging in. Used to store the "logged in" person.
	public class UserModel
	{
		public required int Id { get; set; }
		public required string Email { get; set; }
	}
}
