using EduTechSpace.Context;
using EduTechSpace.Dto;
using EduTechSpace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduTechSpace.Services
{
	public class AdminService
	{
		private readonly RoleManager<IdentityRole<int>> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly EduHubDbContext _eduHubDbContext;
		public AdminService(RoleManager<IdentityRole<int>> roleManager,UserManager<ApplicationUser> userManager
			,EduHubDbContext eduHubDbContext)
		{
			this._roleManager = roleManager;
			this._userManager = userManager;
			this._eduHubDbContext = eduHubDbContext;
		}

		public async Task<AdminDto> CreateNewRole(string roleName)
		{
			bool exists = await _roleManager.RoleExistsAsync(roleName);
			var result = new AdminDto();
			if (string.IsNullOrEmpty(roleName))
			{
				result.Success = false;
				result.Errors?.Add("Empty Role");
				return result;
			}
			if (exists)
			{
				result.Success = false;
				result.Errors?.Add($"Role {roleName} Already Exists in DataBase");
				return result;
			}
			IdentityRole<int> identityRole = new IdentityRole<int>{
				Name = roleName
			};
			var addRoleResult =  await _roleManager.CreateAsync(identityRole);
			result.Success = addRoleResult.Succeeded;
			if(!result.Success)
			{
				foreach (var error in addRoleResult?.Errors)
				{
					result.Errors?.Add(error.Description);
				}
			}
			else
			{
				result.Message = $"Role {roleName} added Successfully";
			}
			return result;
		}
		/// <summary>
		/// Assigns a role to the user identified by their National ID (SSN).
		/// </summary>
		public async Task<AdminDto> AssignRoleToUser(AssignRoleDto dto)
		{
			var result = new AdminDto { Errors = new List<string>() };

			// ── Validate role exists ─────────────────────────────────────────
			bool roleExists = await _roleManager.RoleExistsAsync(dto.RoleName);
			if (!roleExists)
			{
				result.Success = false;
				result.Errors.Add($"Role '{dto.RoleName}' does not exist. " +
					"Please create the role first using the add-role endpoint.");
				return result;
			}

			// ── Look up user by NationalId (SSN) ─────────────────────────────
			var userFromDB = await _eduHubDbContext.Users
				.FirstOrDefaultAsync(u => u.NationalId == dto.NationalId);

			if (userFromDB is null)
			{
				result.Success = false;
				result.Errors.Add($"No user found with National ID (SSN) '{dto.NationalId}'. " +
					"Please verify the NationalId and try again.");
				return result;
			}

			// ── Check user is not already in the role ─────────────────────────
			bool alreadyInRole = await _userManager.IsInRoleAsync(userFromDB, dto.RoleName);
			if (alreadyInRole)
			{
				result.Success = false;
				result.Errors.Add($"User '{userFromDB.FullName ?? userFromDB.UserName}' " +
					$"(NationalId: {dto.NationalId}) is already assigned to role '{dto.RoleName}'.");
				return result;
			}

			try
			{
				var addUserToRole = await _userManager.AddToRoleAsync(userFromDB, dto.RoleName);
				result.Success = addUserToRole.Succeeded;

				if (!addUserToRole.Succeeded)
				{
					foreach (var error in addUserToRole.Errors)
						result.Errors.Add(error.Description);
				}
				else
				{
					result.Message = $"Role '{dto.RoleName}' successfully assigned to user " +
						$"'{userFromDB.FullName ?? userFromDB.UserName}' " +
						$"(NationalId: {dto.NationalId})";
				}
				return result;
			}
			catch (Exception ex)
			{
				result.Errors.Add($"An unexpected error occurred while assigning the role: {ex.Message}");
				result.Success = false;
				return result;
			}
		}
		public async Task<AdminDto> GetAllUsers()
		{
			var response = new AdminDto();

			response.users = await _eduHubDbContext.Users
				.Select(u => new ApplicationUser
				{
					Id = u.Id,
					UserName = u.UserName ?? "",
					Email = u.Email ?? ""
				})
				.ToListAsync();

			return response;
		}
	}
}
