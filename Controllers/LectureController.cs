using EduTechSpace.Dto;
using EduTechSpace.Errors;
using EduTechSpace.interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduTechSpace.Controllers
{
	[Route("api/Lectures")]
	[ApiController]
	public class LectureController : ControllerBase
	{
		private readonly ILectureService _lectureService;
		private CookieOptions _cookieOptions;
		public LectureController(ILectureService lectureService)
		{
			_lectureService = lectureService;
			_cookieOptions = new CookieOptions
			{
				Expires = DateTime.Now.AddDays(7)
			};
		}

		/// <summary>
		/// Admin only – returns ALL lectures in the system.
		/// </summary>
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> GetAll()
		{
			var result = await _lectureService.GetAllAsync();

			if (result == null || !result.Any())
				return NotFound(new ApiResponse(404, "No lectures found."));

			return Ok(result);
		}

		/// <summary>
		/// Teacher / Student – returns only lectures for courses
		/// the current user is enrolled in. UserId is read from Cookie["Id"].
		/// </summary>
		[HttpGet("my")]
		[Authorize(Roles = "Teacher,Student")]
		public async Task<ActionResult> GetMyLectures()
		{
			if (!Request.Cookies.TryGetValue("Id", out var userIdStr) ||
				!int.TryParse(userIdStr, out var userId) || userId <= 0)
			{
				return BadRequest(new ApiResponse(400,
					"User ID cookie is missing or invalid. Please log in again."));
			}

			var result = await _lectureService.GetByUserIdAsync(userId);

			if (result == null || !result.Any())
				return NotFound(new ApiResponse(404, "No lectures found for your courses."));

			return Ok(result);
		}

		/// <summary>
		/// All authenticated roles – returns a single lecture by its ID.
		/// </summary>
		[HttpGet("{id:int}")]
		[Authorize(Roles = "Admin,Teacher,Student")]
		public async Task<ActionResult> GetById(int id)
		{
			if (id <= 0)
				return BadRequest(new ApiResponse(400, "Lecture ID must be greater than zero."));

			var result = await _lectureService.GetByIdAsync(id);

			if (result == null)
				return NotFound(new ApiResponse(404, $"Lecture with ID {id} was not found."));

			Response.Cookies.Append("LectureId", result.Id.ToString(), _cookieOptions);
			return Ok(result);
		}

		/// <summary>
		/// All authenticated roles – returns all lectures for a specific course.
		/// </summary>
		[HttpGet("course/{courseId:int}")]
		[Authorize(Roles = "Admin,Teacher,Student")]
		public async Task<ActionResult> GetByCourseId(int courseId)
		{
			if (courseId <= 0)
				return BadRequest(new ApiResponse(400, "Course ID must be greater than zero."));

			var result = await _lectureService.GetByCourseIdAsync(courseId);

			if (result == null || !result.Any())
				return NotFound(new ApiResponse(404, $"No lectures found for course with ID {courseId}."));

			return Ok(result);
		}

		/// <summary>
		/// Admin / Teacher – creates a new lecture.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult> Create(AddLectureDto dto)
		{
			if (dto == null)
				return BadRequest(new ApiResponse(400, "Lecture payload cannot be null."));

			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse(400, "Invalid lecture data. Please check the request body."));

			var id = await _lectureService.CreateAsync(dto);

			if (id <= 0)
				return BadRequest(new ApiResponse(400, "Failed to create lecture. Please verify the Course ID exists."));

			return CreatedAtAction(nameof(GetById), new { id }, new { Id = id, Message = "Lecture created successfully." });
		}

		/// <summary>
		/// Admin / Teacher – updates an existing lecture.
		/// </summary>
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult> Update(int id, [FromBody] AddLectureDto dto)
		{
			if (id <= 0)
				return BadRequest(new ApiResponse(400, "Lecture ID must be greater than zero."));

			if (dto == null)
				return BadRequest(new ApiResponse(400, "Lecture payload cannot be null."));

			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse(400, "Invalid lecture data. Please check the request body."));

			var result = await _lectureService.UpdateAsync(id, dto);

			if (!result)
				return NotFound(new ApiResponse(404, $"Lecture with ID {id} was not found."));

			return Ok(new { Message = "Lecture updated successfully." });
		}

		/// <summary>
		/// Admin / Teacher – deletes a lecture.
		/// </summary>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult> Delete(int id)
		{
			if (id <= 0)
				return BadRequest(new ApiResponse(400, "Lecture ID must be greater than zero."));

			var result = await _lectureService.DeleteAsync(id);

			if (!result)
				return NotFound(new ApiResponse(404, $"Lecture with ID {id} was not found."));

			return Ok(new { Message = "Lecture deleted successfully." });
		}
	}
}
