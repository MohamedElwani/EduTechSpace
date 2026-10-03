using EduTechSpace.Dto;
using EduTechSpace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace EduTechSpace.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly AccountService _accountService;

		public AccountController(AccountService accountService)
		{
			this._accountService = accountService;
		}
		[HttpPost("register")]
		public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto userFromRequest)
		{
			var authResponse = new AuthResponseDto();
			if (userFromRequest == null)
			{
				authResponse.Message ="Error : User is Empty ";
			}
			authResponse = await _accountService.Register(userFromRequest);
			if (authResponse.Message.Contains("Done"))
			{
				return Ok(value: authResponse);
			}
			
			return BadRequest(authResponse);
		}

		[HttpPost("Login")]
		public async Task<ActionResult<AuthResponseDto>> Login(LoginDto userFromRequest)
		{
			var authResponse = new AuthResponseDto();
			if (userFromRequest == null)
			{
				authResponse.Message= "Error : User is Empty ";
			}
			authResponse = await _accountService.Login(userFromRequest);
			if (authResponse.accessToken == null)
			{
				return BadRequest(authResponse);
			}
			CookieOptions options = new()
			{
				HttpOnly = true,
				Secure = true,
				Expires = DateTime.Now.AddDays(7)
			};
			HttpContext.Response.Cookies.Append("Id",authResponse.UserId.ToString(),options);
			authResponse.UserId = null;// to ignore serilization
			return Ok(authResponse);
		}
		[HttpPost("refresh-token")]
		public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
		{
			if (request == null || string.IsNullOrEmpty(request.RefreshToken))
			{
				return BadRequest(new AuthResponseDto { Message = "Refresh token is required." });
			}

			var authResponse = await _accountService.RefreshTokenAsync(request);
			if (authResponse.accessToken == null)
			{
				return BadRequest(authResponse);
			}

			return Ok(authResponse);
		}

		[Authorize]
		[HttpGet("checkAuth")]
		public IActionResult CheckAuth()
		{
			return Ok("Auth work");
		}
	}
}
