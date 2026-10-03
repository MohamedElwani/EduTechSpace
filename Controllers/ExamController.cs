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
	[Route("api/Exams")]
	[ApiController]
	public class ExamController : ControllerBase
	{
		private readonly IExamService _examService;

		public ExamController(IExamService examService)
		{
			_examService = examService;
		}

		/// <summary>
		/// Admin only – returns ALL exams in the system.
		/// </summary>
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<ExamDto>>> GetAllExams()
		{
			var exams = await _examService.GetAllExamsAsync();

			if (exams == null || !exams.Any())
				return NotFound(new ApiResponse(404, "No exams found."));

			return Ok(exams);
		}

		/// <summary>
		/// Teacher / Student – returns only the exams related to the courses 
		/// the current user is enrolled in. The userId is read from Cookie["Id"].
		/// </summary>
		[HttpGet("my")]
		[Authorize(Roles = "Teacher,Student")]
		public async Task<ActionResult<IEnumerable<ExamDto>>> GetMyExams()
		{
			if (!Request.Cookies.TryGetValue("Id", out var userIdStr) ||
				!int.TryParse(userIdStr, out var userId) || userId <= 0)
			{
				return BadRequest(new ApiResponse(400,
					"User ID cookie is missing or invalid. Please log in again."));
			}

			var exams = await _examService.GetAllExamsByInstructorIdAsync(userId);

			if (exams == null || !exams.Any())
				return NotFound(new ApiResponse(404, "No exams found for your courses."));

			return Ok(exams);
		}

		/// <summary>
		/// All authenticated roles – returns a single exam by its ID.
		/// </summary>
		[HttpGet("{id:int}")]
		[Authorize(Roles = "Admin,Teacher,Student")]
		public async Task<ActionResult<ExamDto>> GetExamById(int id)
		{
			if (id <= 0)
				return BadRequest(new ApiResponse(400, "Exam ID must be greater than zero."));

			var exam = await _examService.GetExamByIdAsync(id);

			if (exam == null)
				return NotFound(new ApiResponse(404, $"Exam with ID {id} was not found."));

			return Ok(exam);
		}

		/// <summary>
		/// All authenticated roles – returns all exams for a specific course.
		/// </summary>
		[HttpGet("course/{courseId:int}")]
		[Authorize(Roles = "Admin,Teacher,Student")]
		public async Task<ActionResult<IEnumerable<ExamDto>>> GetAllExamsByCourseId(int courseId)
		{
			if (courseId <= 0)
				return BadRequest(new ApiResponse(400, "Course ID must be greater than zero."));

			var exams = await _examService.GetAllExamsByCourseIdAsync(courseId);

			if (exams == null || !exams.Any())
				return NotFound(new ApiResponse(404, "This course has no exams."));

			return Ok(exams);
		}

		/// <summary>
		/// Admin / Teacher – creates a new exam.
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<DataResponseDto<ExamDto>>> AddExam(AddExamDto examDto)
		{
			if (examDto == null)
				return BadRequest(new ApiResponse(400, "Exam payload cannot be null."));

			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse(400, "Invalid exam data. Please check the request body."));

			var response = await _examService.AddExamAsync(examDto);

			if (response.Success == true)
				return Ok(response);

			return BadRequest(new ApiResponse(400, response.Errors.FirstOrDefault() ?? "Failed to create exam."));
		}

		/// <summary>
		/// Admin / Teacher – updates an existing exam.
		/// </summary>
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<DataResponseDto<ExamDto>>> UpdateExam(int id, ExamDto examDto)
		{
			if (id <= 0)
				return BadRequest(new ApiResponse(400, "Invalid Exam ID provided in the URL."));

			if (examDto == null)
				return BadRequest(new ApiResponse(400, "Exam payload cannot be null."));

			if (id != examDto.Id)
				return BadRequest(new ApiResponse(400, "ID in the URL does not match the ID in the request body."));

			if (!ModelState.IsValid)
				return BadRequest(new ApiResponse(400, "Invalid exam data. Please check the request body."));

			var response = await _examService.UpdateExamAsync(examDto);

			if (response.Success == true)
				return Ok(response);

			return BadRequest(new ApiResponse(400, response.Errors.FirstOrDefault() ?? "Failed to update exam."));
		}

		/// <summary>
		/// Admin / Teacher – deletes an exam (fails if it has results).
		/// </summary>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<DataResponseDto<ExamDto>>> DeleteExam(int id)
		{
			if (id <= 0)
				return BadRequest(new ApiResponse(400, "Exam ID must be greater than zero."));

			var response = await _examService.DeleteExamAsync(id);

			if (response.Success == true)
				return Ok(response);

			return BadRequest(new ApiResponse(400, response.Errors.FirstOrDefault() ?? "Failed to delete exam."));
		}

		/// <summary>
		/// Admin only – deletes an exam along with all its student results.
		/// </summary>
		[HttpDelete("{id}/with-results")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<DataResponseDto<ExamDto>>> DeleteExamWithResults(int id)
		{
			if (id <= 0)
				return BadRequest(new ApiResponse(400, "Exam ID must be greater than zero."));

			var response = await _examService.DeleteExamWithResultsAsync(id);

			if (response.Success == true)
				return Ok(response);

			return BadRequest(new ApiResponse(400, response.Errors.FirstOrDefault() ?? "Failed to delete exam with results."));
		}
	}
}