using EduTechSpace.Dto;
using EduTechSpace.Errors;
using EduTechSpace.interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduTechSpace.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StudentResultsController : ControllerBase
	{
		private readonly IExamResultService _examResultService;

		public StudentResultsController(IExamResultService examResultService)
		{
			_examResultService = examResultService;
		}

		/// <summary>
		/// Admin / Teacher – returns all exam results for a specific course.
		/// </summary>
		[HttpGet("course/{courseId}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<IEnumerable<ExamWithExamResultsDto>>> GetAllByCourseId(int courseId)
		{
			if (courseId <= 0)
				return BadRequest(new ApiResponse(400, "Course ID must be greater than zero."));

			var results = await _examResultService.GetAllExamsWithResultsByCourseIdAsync(courseId);

			if (results == null || !results.Any())
				return NotFound(new ApiResponse(404, $"No exam results found for course with ID {courseId}."));

			return Ok(results);
		}

		/// <summary>
		/// Admin only – returns all exam results for a specific instructor by ID.
		/// </summary>
		[HttpGet("instructor/{instructorId}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<ExamWithExamResultsDto>>> GetAllByInstructorId(int instructorId)
		{
			if (instructorId <= 0)
				return BadRequest(new ApiResponse(400, "Instructor ID must be greater than zero."));

			var results = await _examResultService.GetAllExamsWithResultsByInstructorIdAsync(instructorId);

			if (results == null || !results.Any())
				return NotFound(new ApiResponse(404, $"No exam results found for instructor with ID {instructorId}."));

			return Ok(results);
		}

		/// <summary>
		/// Teacher – returns exam results for the current teacher's courses.
		/// The userId is read from Cookie["Id"].
		/// </summary>
		[HttpGet("my")]
		[Authorize(Roles = "Teacher")]
		public async Task<ActionResult<IEnumerable<ExamWithExamResultsDto>>> GetMyResults()
		{
			if (!Request.Cookies.TryGetValue("Id", out var userIdStr) ||
				!int.TryParse(userIdStr, out var userId) || userId <= 0)
			{
				return BadRequest(new ApiResponse(400,
					"User ID cookie is missing or invalid. Please log in again."));
			}

			var results = await _examResultService.GetAllExamsWithResultsByInstructorIdAsync(userId);

			if (results == null || !results.Any())
				return NotFound(new ApiResponse(404, "No exam results found for your courses."));

			return Ok(results);
		}

		/// <summary>
		/// Admin / Teacher – returns all exam results for a specific student by ID.
		/// </summary>
		[HttpGet("student/{studentId}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<IEnumerable<ExamWithExamResultsDto>>> GetAllByStudentId(int studentId)
		{
			if (studentId <= 0)
				return BadRequest(new ApiResponse(400, "Student ID must be greater than zero."));

			var results = await _examResultService.GetAllExamsWithResultsByStudentIdAsync(studentId);

			if (results == null || !results.Any())
				return NotFound(new ApiResponse(404, $"No exam results found for student with ID {studentId}."));

			return Ok(results);
		}

		/// <summary>
		/// Student – returns the current student's own exam results.
		/// The userId is read from Cookie["Id"].
		/// </summary>
		[HttpGet("my-results")]
		[Authorize(Roles = "Student")]
		public async Task<ActionResult<IEnumerable<ExamWithExamResultsDto>>> GetMyStudentResults()
		{
			if (!Request.Cookies.TryGetValue("Id", out var userIdStr) ||
				!int.TryParse(userIdStr, out var userId) || userId <= 0)
			{
				return BadRequest(new ApiResponse(400,
					"User ID cookie is missing or invalid. Please log in again."));
			}

			var results = await _examResultService.GetAllExamsWithResultsByStudentIdAsync(userId);

			if (results == null || !results.Any())
				return NotFound(new ApiResponse(404, "No exam results found for your account."));

			return Ok(results);
		}

		/// <summary>
		/// Admin / Teacher – adds a new exam result for a student.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<DataResponseDto<ExamResultDto>>> AddExamResult(AddExamResultDto examResultDto)
		{
			if (examResultDto == null)
				return BadRequest(new ApiResponse(400, "Exam result payload cannot be null."));

			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse(400, "Invalid exam result data. Please check the request body."));

			var response = await _examResultService.AddExamResult(examResultDto);

			if (response.Success == true)
				return Ok(response);

			return BadRequest(new ApiResponse(400, response.Errors.FirstOrDefault() ?? "Failed to add exam result."));
		}

		/// <summary>
		/// Admin / Teacher – updates an existing exam result.
		/// </summary>
		[HttpPut]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<DataResponseDto<ExamDto>>> UpdateExamResult([FromBody] ExamResultDto examResultDto)
		{
			if (examResultDto == null)
				return BadRequest(new ApiResponse(400, "Exam result payload cannot be null."));

			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse(400, "Invalid exam result data. Please check the request body."));

			var response = await _examResultService.UpdateExamResult(examResultDto);

			if (response.Success == true)
				return Ok(response);

			return BadRequest(new ApiResponse(400, response.Errors.FirstOrDefault() ?? "Failed to update exam result."));
		}

		/// <summary>
		/// Admin / Teacher – deletes all results for a specific exam.
		/// </summary>
		[HttpDelete("exam/{examId}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<DataResponseDto<ExamDto>>> DeleteExamResults(int examId)
		{
			if (examId <= 0)
				return BadRequest(new ApiResponse(400, "Exam ID must be greater than zero."));

			var response = await _examResultService.DeleteExamResult(examId);

			if (response.Success == true)
				return Ok(response);

			return BadRequest(new ApiResponse(400, response.Errors.FirstOrDefault() ?? "Failed to delete exam results."));
		}
	}
}