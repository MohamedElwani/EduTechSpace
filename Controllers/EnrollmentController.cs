using EduTechSpace.Dto;
using EduTechSpace.Errors;
using EduTechSpace.interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduTechSpace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/enrollment
        // • Admin  → all enrollments in the system
        // • Teacher → only enrollments for courses they own
        //   (service reads the caller's Id from the JWT and filters internally)
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        //[Authorize(Roles = "Teacher,admin")]
        public async Task<ActionResult<EnrollmentResponseDto>> GetAll()
        {
            var result = await _enrollmentService.GetAllEnrollmentsAsync();

            if (!result.Success)
                return NotFound(new ApiResponse(404, result.Errors.FirstOrDefault()
                    ?? "No enrollments found."));

            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/enrollment/user/{userId}
        // Returns all courses a specific user is enrolled in.
        // A Teacher can only look up their own userId or a student enrolled in
        // one of their courses; admin can look up anyone.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("user/{userId:int}")]
        //[Authorize(Roles = "Teacher,admin,student")]
        public async Task<ActionResult<UserEnrollmentsResponseDto>> GetByUser(int userId)
        {
            if (userId <= 0)
                return BadRequest(new ApiResponse(400,
                    "UserId must be a positive integer greater than zero."));

            var result = await _enrollmentService.GetEnrollmentsByUserIdAsync(userId);

            if (!result.Success)
                return NotFound(new ApiResponse(404, result.Errors.FirstOrDefault()
                    ?? $"No enrollments found for UserId '{userId}'."));

            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/enrollment/course/{courseId}
        // Returns all STUDENTS enrolled in a specific course.
        // Teachers are excluded from the list automatically.
        // A Teacher caller must own this course; admin can query any course.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("course/{courseId:int}")]
        //[Authorize(Roles = "Teacher,admin")]
        public async Task<ActionResult<CourseEnrollmentsResponseDto>> GetByCourse(int courseId)
        {
            if (courseId <= 0)
                return BadRequest(new ApiResponse(400,
                    "CourseId must be a positive integer greater than zero."));

            var result = await _enrollmentService.GetEnrollmentsByCourseIdAsync(courseId);

            if (!result.Success)
                return NotFound(new ApiResponse(404, result.Errors.FirstOrDefault()
                    ?? $"No course found with Id '{courseId}'."));

            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/enrollment/user/{userId}/course/{courseId}
        // Returns a single enrollment record by composite key.
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("user/{userId:int}/course/{courseId:int}")]
        //[Authorize(Roles = "Teacher,admin,student")]
        public async Task<ActionResult<DataResponseDto<EnrollmentResponseItemDto>>> GetById(
            int userId, int courseId)
        {
            if (userId <= 0)
                return BadRequest(new ApiResponse(400,
                    "UserId must be a positive integer greater than zero."));

            if (courseId <= 0)
                return BadRequest(new ApiResponse(400,
                    "CourseId must be a positive integer greater than zero."));

            var result = await _enrollmentService.GetEnrollmentByIdAsync(userId, courseId);

            if (!result.Success ?? false)
                return NotFound(new ApiResponse(404, result.Errors.FirstOrDefault()
                    ?? $"No enrollment record found for UserId '{userId}' and CourseId '{courseId}'."));

            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/enrollment/check?userId={}&courseId={}
        // Simple bool check — is the user enrolled in this course?
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("check")]
        //[Authorize(Roles = "Teacher,admin,student")]
        public async Task<ActionResult<object>> IsEnrolled(
            [FromQuery] int userId, [FromQuery] int courseId)
        {
            if (userId <= 0)
                return BadRequest(new ApiResponse(400,
                    "UserId must be a positive integer greater than zero."));

            if (courseId <= 0)
                return BadRequest(new ApiResponse(400,
                    "CourseId must be a positive integer greater than zero."));

            var isEnrolled = await _enrollmentService.IsUserEnrolledAsync(userId, courseId);

            return Ok(new
            {
                Success = true,
                IsEnrolled = isEnrolled,
                Message = isEnrolled
                    ? $"User '{userId}' IS enrolled in course '{courseId}'."
                    : $"User '{userId}' is NOT enrolled in course '{courseId}'."
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET  api/enrollment/count/{courseId}
        // Total number of students enrolled in a course (teachers excluded).
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("count/{courseId:int}")]
        //[Authorize(Roles = "Teacher,admin")]
        public async Task<ActionResult<object>> GetEnrollmentCount(int courseId)
        {
            if (courseId <= 0)
                return BadRequest(new ApiResponse(400,
                    "CourseId must be a positive integer greater than zero."));

            var count = await _enrollmentService.GetEnrollmentCountByCourseIdAsync(courseId);

            // Service returns -1 when the course does not exist
            if (count == -1)
                return NotFound(new ApiResponse(404,
                    $"Course with Id '{courseId}' was not found. Please provide a valid CourseId."));

            return Ok(new
            {
                Success = true,
                CourseId = courseId,
                TotalEnrollments = count,
                Message = $"Course '{courseId}' has {count} enrolled student(s)."
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST  api/enrollment
        // Enroll a user (student) in a course.
        // Teacher callers can only enroll students into their own courses.
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        //[Authorize(Roles = "Teacher,admin")]
        public async Task<ActionResult<EnrollmentResponseDto>> Enroll([FromBody] EnrollmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse(400, string.Join(" | ", modelErrors)));
            }

            var result = await _enrollmentService.EnrollUserAsync(dto);

            if (!result.Success)
            {
                var errorMessage = result.Errors.FirstOrDefault() ?? "Enrollment failed.";

                // Duplicate enrollment → 409 Conflict
                if (errorMessage.Contains("already enrolled", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new ApiResponse(409, errorMessage));

                // Missing resource → 404
                if (errorMessage.Contains("no user found", StringComparison.OrdinalIgnoreCase)
                    || errorMessage.Contains("no course found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new ApiResponse(404, errorMessage));

                return BadRequest(new ApiResponse(400, errorMessage));
            }

            return StatusCode(201, result);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE  api/enrollment/user/{userId}/course/{courseId}
        // Unenroll a student from a course.
        // Teacher callers can only unenroll from their own courses.
        // ─────────────────────────────────────────────────────────────────────
        [HttpDelete("user/{userId:int}/course/{courseId:int}")]
        //[Authorize(Roles = "Teacher,admin")]
        public async Task<ActionResult<EnrollmentResponseDto>> Unenroll(int userId, int courseId)
        {
            if (userId <= 0)
                return BadRequest(new ApiResponse(400,
                    "UserId must be a positive integer greater than zero."));

            if (courseId <= 0)
                return BadRequest(new ApiResponse(400,
                    "CourseId must be a positive integer greater than zero."));

            var result = await _enrollmentService.UnenrollUserAsync(userId, courseId);

            if (!result.Success)
            {
                var errorMessage = result.Errors.FirstOrDefault() ?? "Unenrollment failed.";

                if (errorMessage.Contains("no enrollment record found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new ApiResponse(404, errorMessage));

                return BadRequest(new ApiResponse(400, errorMessage));
            }

            return Ok(result);
        }
    }
}
