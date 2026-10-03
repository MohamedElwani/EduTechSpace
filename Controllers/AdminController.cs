using EduTechSpace.Dto;
using EduTechSpace.Errors;
using EduTechSpace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTechSpace.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class AdminController : ControllerBase
	{
		private readonly AdminService _adminService;
		public AdminController(AdminService adminService) { 
		 this._adminService = adminService;
		}
		[HttpPost("add-role/{role:alpha}")]
		public async Task<ActionResult<AdminDto>> AddRole(string role)
		{
			AdminDto Response = await _adminService.CreateNewRole(role);
			if (Response.Success)
			{
				return Ok(Response);
			}
			return BadRequest(Response);
		}
		// ─────────────────────────────────────────────────────────────────────
		// POST  api/admin/assign-role
		// Assigns a role to the user identified by their NationalId (SSN).
		// ─────────────────────────────────────────────────────────────────────
		[HttpPost("assign-role")]
		public async Task<ActionResult<AdminDto>> AssignRoleToUser([FromBody] AssignRoleDto dto)
		{
			if (!ModelState.IsValid)
			{
				var modelErrors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToList();

				return BadRequest(new ApiResponse(400, string.Join(" | ", modelErrors)));
			}

			AdminDto response = await _adminService.AssignRoleToUser(dto);

			if (!response.Success)
			{
				var errorMsg = response.Errors?.FirstOrDefault() ?? "Failed to assign role.";

				// NationalId not found or role not found → 404
				if (errorMsg.Contains("No user found", StringComparison.OrdinalIgnoreCase)
					|| errorMsg.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
					return NotFound(new ApiResponse(404, errorMsg));

				// Already in role → 409 Conflict
				if (errorMsg.Contains("already assigned", StringComparison.OrdinalIgnoreCase))
					return Conflict(new ApiResponse(409, errorMsg));

				return BadRequest(new ApiResponse(400, errorMsg));
			}

			return Ok(response);
		}
		[HttpGet("get-all-users")]
		public async Task<ActionResult<AdminDto>> GetAllUsers()
		{
			AdminDto Response = await _adminService.GetAllUsers();
			if (Response.Success)
			{
				return Ok(Response);
			}
			return BadRequest(Response);
		}
	}
}
