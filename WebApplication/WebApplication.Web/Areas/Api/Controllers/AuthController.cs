using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Domain.Models;
using WebApplication.Infrastructure.Repos.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication.Shared.Helpers;
namespace WebApplication.App.Web.Areas.Api.Controllers
{
	[Area("Api")]
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IUserRepository _userRepo;
		private readonly IConfiguration _config;

		public AuthController(IUserRepository userRepo, IConfiguration config)
		{
			_userRepo = userRepo;
			_config = config;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			var existingEmail = await _userRepo.GetByEmailAsync(request.Email);
			var existingUsername = await _userRepo.GetByEmailAsync(request.Username);

			if (existingEmail != null)
				return BadRequest("Email already exists.");
			
			if (existingUsername != null)
				return BadRequest("Username already exists.");

			if (!EmailValidation.IsValid(request.Email))
			{
				return BadRequest("Invalid email format.");
			}
			var user = new User
			{
				Email = request.Email,
				Username = request.Username,
				PasswordHash = HashPassword(request.Password)
			};

			await _userRepo.AddAsync(user);
			return Ok("User registered.");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Email))
				return BadRequest("Email is required");

			User? user = null;
			if (request.Email.Contains("@"))
			{
				user = await _userRepo.GetByEmailAsync(request.Email);
			}
			else
			{
				user = await _userRepo.GetByUsernameAsync(request.Email);
			}

			if (user == null || user.PasswordHash != HashPassword(request.Password))
				return Unauthorized("Invalid credentials");

			var token = GenerateJwtToken(user);

			return Ok(new { token });
		}

		// Helper: Generate JWT token
		private string GenerateJwtToken(User user)
		{
			var jwt = _config.GetSection("Jwt");
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
		};

			var token = new JwtSecurityToken(
				issuer: jwt["Issuer"],
				audience: jwt["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwt["ExpiresInMinutes"])),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		// Helper: Very basic hash
		private string HashPassword(string password)
		{
			using var sha = SHA256.Create();
			var bytes = Encoding.UTF8.GetBytes(password);
			var hash = sha.ComputeHash(bytes);
			return Convert.ToBase64String(hash);
		}
	}
	public class RegisterRequest
	{
		public string Email { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}

	public class LoginRequest
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}
