using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.IdentityModel.Tokens;
using MyBlazor.Server.Database;
using MyBlazor.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace MyBlazor.Server.Controllers
{
	[ApiController]
	[Route("account")]
	public class AuthenticationController : ControllerBase
	{
		private readonly ILogger<AuthenticationController> _logger;
		private readonly UsersContext _context;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IConfiguration _configuration;

		public AuthenticationController(
			UsersContext context,
			ILogger<AuthenticationController> logger,
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			IConfiguration configuration)
		{
			_context = context;
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_configuration = configuration;
		}

		private string GenerateJwtToken(User user)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				//new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				//new Claim(JwtRegisteredClaimNames.Email, user.Email),
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddHours(1),
				//expires: DateTime.Now.AddSeconds(10),
				signingCredentials: creds
			);

			var tokenHandler = new JwtSecurityTokenHandler();
			return tokenHandler.WriteToken(token);
		}

		[HttpPost, Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginRegisterModel data)
		{
			var user = await _userManager.FindByEmailAsync(data.Email);
			if (user != null && await _userManager.CheckPasswordAsync(user, data.Password))
			{
				_logger.Log(LogLevel.Information, "Logging in to account: {0}", data.Email);
				return Ok(new
				{
					JWTToken = GenerateJwtToken(user)
				});
			}
			return Unauthorized();
		}

		[HttpPost, Route("register")]
		public async Task<IActionResult> Register([FromBody] LoginRegisterModel data)
		{
			var user = await _userManager.FindByEmailAsync(data.Email);
			if (user != null)
				return BadRequest(new { Message = "Invalid email or password!" });

			user = new User
			{
				Name = "",
				UserName = data.Email,
				Email = data.Email
			};

			var identity = await _userManager.CreateAsync(user, data.Password);
			if (identity.Succeeded)
			{
				_logger.Log(LogLevel.Information, "Account registered successfully: ID: {0}, Email: {0}", user.Id, data.Email);
				return Ok();
			}
			else
			{
				RegisterResultModel registerResultModel = new RegisterResultModel
				{
					Errors = identity.Errors.Select(err => err.Description).ToList()
				};

				return BadRequest(registerResultModel);
			}
		}

		[HttpGet, Route("auth_test")]
		[OutputCache(NoStore = true)]
		public async Task<IActionResult> AuthTest()
		{
			if (HttpContext.User is not null && HttpContext.User.Identity.IsAuthenticated)
			{

			}
			return Ok();
		}

		[HttpPost, Route("validate_token")]
		[Authorize, OutputCache(NoStore = true)]
		public async Task<IActionResult> ValidateToken()
		{
			// Authorization happens implicitly with [Authorize], so this code only runs if user is authenticated
			var email = HttpContext.User.Claims.Where(claim => claim.Type == ClaimTypes.Email).FirstOrDefault();
			if (email is not null)
			{
				var user = await _userManager.FindByEmailAsync(email.Value);
				if (user is not null && user.Email is not null)
				{
					return Ok();
				}
			}
			return BadRequest();
		}

		[HttpPost, Route("refresh_token")]
		[Authorize, OutputCache(NoStore = true)]
		public async Task<IActionResult> RefreshToken()
		{
			if (HttpContext.User is not null)
			{
				var email = HttpContext.User.Claims.Where(claim => claim.Type == ClaimTypes.Email).FirstOrDefault();
				if (email is not null)
				{
					var user = await _userManager.FindByEmailAsync(email.Value);
					if (user is not null && user.Email is not null)
					{
						return Ok(new
						{
							Token = GenerateJwtToken(user)
						});
					}
				}
			}
			return BadRequest();
		}
	}
}