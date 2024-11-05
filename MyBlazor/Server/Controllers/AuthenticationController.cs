using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MyBlazor.Server.Database;
using MyBlazor.Server.Services;
using MyBlazor.Shared;
using MyBlazor.SharedUI.Pages.Models;

namespace MyBlazor.Server.Controllers
{
	[ApiController]
	[Route("account")]
	public class AuthenticationController : ControllerBase
	{
		private readonly ILogger<AuthenticationController> _logger;
		private readonly UsersContext _context;
		private readonly ISessionTrackerService _sessionTrackerService;

		public AuthenticationController(
			UsersContext context,
			ISessionTrackerService sessionTrackerService,
			ILogger<AuthenticationController> logger)
		{
			_context = context;
			_sessionTrackerService = sessionTrackerService;
			_logger = logger;
		}

		[HttpPost]
		[Route("login")]
		public Shared.User? Login(string email, string codedPassword)
		{
			var user = _context.Users.Where(usr => usr.Email == email && usr.Password == codedPassword).FirstOrDefault();
			if (user == null)
			{
				throw new Exception("Invalid email or password!");
			}

			var newSession = _sessionTrackerService.CreateNewSession(user.Id);
			newSession.Match(
				SID =>
				{
					CookieOptions options = new CookieOptions();
					options.Expires = DateTime.Now.AddDays(1);

					HttpContext.Response.Cookies.Append("SID", SID, options);
				},
				error =>
				{
					throw new Exception("Invalid email or password!");
				}
			);

			return user;
		}

		[HttpPost]
		[Route("register")]
		public int Register([FromBody]RegisterModel data)
		{
			var user = _context.Users.Where(usr => usr.Email == data.Email).FirstOrDefault();
			if (user != null)
				throw new Exception("Invalid email or password!");

			user = new Database.User
			{
				Name = "",
				Email = data.Email,
				Password = data.Password
			};

			_context.Add(user);
			_context.SaveChanges();

			return user.Id;
		}
	}
}