using EduTechSpace.Context;
using EduTechSpace.Dto;
using EduTechSpace.Models;
using EduTechSpace.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EduTechSpace.Services
{
	public class AccountService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly AdminService _adminService;
		private readonly EduHubDbContext _eduHubDbContext;

		public AccountService(UserManager<ApplicationUser> userManager
			, IConfiguration configuration
,			IWebHostEnvironment webHostEnvironment
			,AdminService adminService
			,EduHubDbContext eduHubDbContext
			)
		{
			this._userManager = userManager;
			this._configuration = configuration;
			this._webHostEnvironment = webHostEnvironment;
			this._adminService = adminService;
			this._eduHubDbContext = eduHubDbContext;
		}
		public async Task<AuthResponseDto> Register(RegisterDto userFromRequest)
		{
			var Response = new AuthResponseDto();
			ApplicationUser user = new ApplicationUser();
			user.FullName = userFromRequest.fullName;
			user.UserName = userFromRequest.userName;
			user.Email = userFromRequest.email;
			user.accountType = userFromRequest.AccountType;
			user.NationalId = userFromRequest.NationalId;
			if (userFromRequest.files != null)
			{
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(userFromRequest.files.FileName);
				string path = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
				using (var stream = new FileStream(path,FileMode.Create))
				{
					await userFromRequest.files.CopyToAsync(stream);
				}
				//user.imgUrl = path; Don't send absolute path send relative one 
				user.imgUrl =  $"images/{fileName}";// relative path 
			}
			IdentityResult identityResult = await _userManager.CreateAsync(user, userFromRequest.password);
			if (identityResult.Succeeded)
			{
				string role = (user.accountType == AccountType.Teacher) ? "Teacher" : "Student";
				var roleDto = new AssignRoleDto { NationalId = user.NationalId, RoleName = role };
				await _adminService.AssignRoleToUser(roleDto);
				//Add To UserYearTable 
				UserYear userYear = new UserYear();
				userYear.UserLevel = userFromRequest.UserLevel;
				userYear.UserId = user.Id;
				await _eduHubDbContext.Set<UserYear>().AddAsync(userYear);
				await _eduHubDbContext.SaveChangesAsync();
				Response.Message ="Done!, User created successfully";
			}
			else
			{
				Response.Message = "Unable to create the user";
				foreach (var error in identityResult.Errors)
				{
					Response.errors.Add(error.Description.ToString());
				}
			}
			return Response;
		}
		public async Task<AuthResponseDto> Login(LoginDto userFromRequest)
		{
			var Response = new AuthResponseDto();
			//var userFromDB = await _userManager.FindByEmailAsync(userFromRequest.email);
			var userFromDB = await _eduHubDbContext.Users.FirstOrDefaultAsync(u=>u.NationalId == userFromRequest.ID );
			if (userFromDB == null) {
				Response.Message = "Error, invalid user contact system admin for more information";
				return Response;
			}
			else if((userFromRequest.isStudent && userFromDB.accountType == AccountType.Teacher)
				|| (!userFromRequest.isStudent && userFromDB.accountType == AccountType.Student))
			{
				Response.errors.Add("User Type doesn't match the type stored in DB");
				return Response;
			}
			Response = await GenerateAccessToken(userFromDB);
			Response.UserId = userFromDB.Id;
			return Response;
			}
		public async Task<AuthResponseDto> GenerateAccessToken(ApplicationUser userFromDB)
		{
			List<Claim> claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.NameIdentifier, userFromDB.Id.ToString()));
			claims.Add(new Claim(ClaimTypes.Name, userFromDB.NormalizedUserName));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString() ));
			//if(userFromDB.UserYears != null)
			//{
			//	foreach(var level in userFromDB.UserYears)
			//	{
			//		claims.Add(new Claim("User Levels", level.UserLevel.ToString()));
			//	}
			//}
			IList<string> Roles = await _userManager.GetRolesAsync(userFromDB);
			foreach (var role in Roles) {
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"]));
			SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issur"],
				expires: DateTime.Now.AddDays(1),
				claims: claims,
				signingCredentials: signingCredentials
			);

			// Generate and store Refresh Token
			var refreshTokenString = GenerateRefreshTokenString();
			var refreshToken = new RefreshToken
			{
				Token = refreshTokenString,
				ExpiresAt = DateTime.UtcNow.AddDays(7), // Configurable, e.g., 7 days
				UserId = userFromDB.Id
			};
			await _eduHubDbContext.RefreshTokens.AddAsync(refreshToken);
			await _eduHubDbContext.SaveChangesAsync();

			return new AuthResponseDto
			{
				accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
				accessTokenExpireDate = DateTime.Now.AddDays(1),
				refreshToken = refreshTokenString,
				refreshTokenExpiresAt = refreshToken.ExpiresAt,
				Message = "User Loggend in Successfully"
			};
		}

		private string GenerateRefreshTokenString()
		{
			var randomNumber = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
		{
			var response = new AuthResponseDto();
			var storedToken = await _eduHubDbContext.RefreshTokens
				.Include(rt => rt.User)
				.FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

			if (storedToken == null)
			{
				response.Message = "Invalid refresh token.";
				return response;
			}

			if (!storedToken.IsActive)
			{
				response.Message = "Refresh token is expired, used, or revoked. Please login again.";
				return response;
			}

			if (storedToken.User == null)
			{
				response.Message = "User associated with token not found.";
				return response;
			}

			// Mark current token as used
			storedToken.IsUsed = true;
			_eduHubDbContext.RefreshTokens.Update(storedToken);
			// Save changes here or let GenerateAccessToken save it. Let's save explicitly in GenerateAccessToken but this is fine.

			// Generate new tokens (this generates a new access token and a new refresh token, and saves all tracked changes)
			var newTokens = await GenerateAccessToken(storedToken.User);
			newTokens.Message = "Tokens refreshed successfully.";
			
			return newTokens;
		}
		
	}
}
