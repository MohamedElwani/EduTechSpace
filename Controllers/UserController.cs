using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EduTechSpace.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet("students")]
		public async Task<IActionResult> GetAllStudents()
		{
			try
			{
				var students = await _userService.GetAllStudentsAsync();

				return Ok(students);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("students/names")]
		public async Task<IActionResult> GetStudentNamesOnly()
		{
			try
			{
				var students = await _userService.GetStudentNamesOnlyAsync();

				return Ok(students);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("students/{id}")]
		public async Task<IActionResult> GetStudentDetails(int id)
		{
			try
			{
				var student = await _userService.GetStudentDetailsAsync(id);

				if (student == null)
					return NotFound("Student not found");

				return Ok(student);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("students/instructor/{instructorId}")]
		public async Task<IActionResult> GetStudentsByInstructor(int instructorId, [FromQuery] Levels? level)
		{
			try
			{
				var students = await _userService.GetStudentsByInstructorAsync(instructorId, level);

				return Ok(students);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("assign-student")]
		public async Task<IActionResult> AddStudentToInstructor(int studentId, int instructorId)
		{
			try
			{
				await _userService.AddStudentToInstructorAsync(studentId, instructorId);

				return Ok("Student assigned successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}