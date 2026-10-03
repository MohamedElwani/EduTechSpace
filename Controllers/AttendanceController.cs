using EduTechSpace.Dto;
using EduTechSpace.Errors;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduTechSpace.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AttendanceController : ControllerBase
	{
		private readonly IAttendanceService _attendanceService;

		public AttendanceController(IAttendanceService attendanceService)
		{
			_attendanceService = attendanceService;
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<IEnumerable<Attendance>>> GetAll()
		{
			var result = await _attendanceService.GetAllAsync();

			if (result.Successed == true)
				return Ok(result);

			return NotFound(new ApiResponse(404));
		}

		[HttpGet("lecture/{lectureId:int}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<ActionResult<Attendance>> GetByLecture(int lectureId)
		{
			var result = await _attendanceService.GetByLectureIdAsync(lectureId);

			if (result.Successed == true && result.attendances?.Count()>0)
				return Ok(result);

			return NotFound(new ApiResponse(404 ,"This Lecture has no attendances"));
		}
		/// <summary>
		///  Get any student attendance sheet only for instructors and admins 
		/// </summary>
		/// <param name="studentId"></param>
		/// <returns></returns>
		[HttpGet("students/{studentId:int}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<IActionResult> GetByStudentId(int studentId)
		{
			var result = await _attendanceService.GetByStudentIdAsync(studentId);

			if (result.Successed == true && result.attendances?.Count()>0)
				return Ok(result);

			return NotFound(new ApiResponse(404, "This student has no attendances"));
		}
		/// <summary>
		///  Get any student attendance sheet for students
		/// </summary>
		/// <param name="studentId"></param>
		/// <returns></returns>
		[HttpGet("student")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> GetStudentAttendance(int studentId)
		{
			studentId = int.Parse(HttpContext.Request.Cookies["Id"]);
			var result = await _attendanceService.GetByStudentIdAsync(studentId);

			if (result.Successed == true && result.attendances?.Count() > 0)
				return Ok(result);

			return NotFound(new ApiResponse(404, "This student has no attendances"));
		}

		[HttpGet("student/{studentId:int}/lecture/{lectureId:int}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<IActionResult> GetStudentInLecture(int studentId, int lectureId)
		{
			var result = await _attendanceService
				.GetStudentAttendanceInLectureAsync(lectureId, studentId);

			if (result.Successed == true)
				return Ok(result);

			return NotFound(new ApiResponse(404, "This student hasn't any attendances at this lecture"));
		}

		[HttpPost("mark")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceDto dto)
		{
			var result = await _attendanceService.MarkAttendanceAsync(dto);

			if (result.Success == true)
				return Ok(result);

			return BadRequest(new ApiResponse(400 ,"Error Accured while saving the attendance"));
		}

		[HttpPatch("update-status/{attendanceId:int}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<IActionResult> UpdateStatus(int attendanceId, [FromQuery] string status)
		{
			var result = await _attendanceService.UpdateStatusAsync(attendanceId, status);

			if (result == true)
				return Ok(new { Success = true, Message = "Status updated" });

			return BadRequest(new ApiResponse(400, "Error Accured while update the attendance"));
		}

		[HttpDelete("delete/{attendanceId:int}")]
		[Authorize(Roles = "Admin,Teacher")]
		public async Task<IActionResult> Delete(int attendanceId)
		{
			var result = await _attendanceService.DeleteAsync(attendanceId);

			if (result == true)
				return Ok(new { Success = true, Message = "Deleted successfully" });

			return BadRequest(new { Success = false, Message = "Not found or failed" });
		}
	}
}
