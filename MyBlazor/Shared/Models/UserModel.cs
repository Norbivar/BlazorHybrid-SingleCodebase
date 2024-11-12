using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazor.Shared.Models
{
	// The information stored on the CLIENT-SIDE. Mainly used to store the "logged in" person.
	public class UserModel
	{
		public required int Id { get; set; }
		public required string Email { get; set; }
	}
}
