using AutoMapper;
using EduTechSpace.Dto;
using EduTechSpace.interfaces;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Models;
using System.Security.Claims;

namespace EduTechSpace.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _enrollmentRepository = enrollmentRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET ALL
        // ─────────────────────────────────────────────────────────────────────
        public async Task<EnrollmentResponseDto> GetAllEnrollmentsAsync()
        {
            var response = new EnrollmentResponseDto();

            try
            {
                List<UserCourse> enrollments;

                // If caller is a Teacher, restrict to courses they own.
                if (IsCurrentUserTeacher())
                {
                    var teacherId = GetCurrentUserId();
                    if (teacherId is null)
                    {
                        response.Success = false;
                        response.Errors.Add("Unable to identify the calling teacher. Please ensure your token is valid.");
                        return response;
                    }
                    // Get all courses owned by this teacher, then fetch students for each.
                    var teacherCourses = await _enrollmentRepository.GetByUserIdAsync(teacherId.Value);
                    var courseIds = teacherCourses.Select(uc => uc.CourseId).Distinct().ToList();

                    var allStudentEnrollments = new List<UserCourse>();
                    foreach (var courseId in courseIds)
                        allStudentEnrollments.AddRange(await _enrollmentRepository.GetByCourseIdAsync(courseId));

                    enrollments = allStudentEnrollments;
                }
                else
                {
                    // Admin: return all (students only – teachers excluded at repo level)
                    enrollments = (await _enrollmentRepository.GetAllAsync())
                        .Where(uc => uc.User != null && uc.User.accountType == AccountType.Student)
                        .ToList();
                }

                response.Enrollments = _mapper.Map<List<EnrollmentResponseItemDto>>(enrollments);
                response.TotalEnrollments = response.Enrollments.Count;
                response.Success = true;
                response.SuccessMessage = $"Retrieved {response.TotalEnrollments} enrollment(s) successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"An unexpected error occurred while retrieving enrollments: {ex.Message}");
            }

            return response;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET BY USER  →  user data once + list of courses
        // ─────────────────────────────────────────────────────────────────────
        public async Task<UserEnrollmentsResponseDto> GetEnrollmentsByUserIdAsync(int userId)
        {
            var response = new UserEnrollmentsResponseDto();

            // ── Validation ──────────────────────────────────────────────────
            if (userId <= 0)
            {
                response.Success = false;
                response.Errors.Add("UserId must be a positive integer greater than zero.");
                return response;
            }

            try
            {
                var user = await _userRepository.GetUserById(userId);
                if (user is null)
                {
                    response.Success = false;
                    response.Errors.Add($"No user found with Id '{userId}'. Please verify the UserId and try again.");
                    return response;
                }

                var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId);

                // Build the summary — user identity appears exactly once
                var summary = new UserEnrollmentsSummaryDto
                {
                    UserId      = user.Id,
                    FullName    = user.FullName ?? user.UserName,
                    AccountType = user.accountType.ToString(),
                    Courses     = enrollments
                        .Where(uc => uc.Course != null)
                        .Select(uc => new EnrolledCourseDto
                        {
                            CourseId    = uc.CourseId,
                            CourseTitle = uc.Course!.Title,
                            CourseCode  = uc.Course!.CourseCode
                        }).ToList()
                };
                summary.TotalCourses = summary.Courses.Count;

                response.UserEnrollments = summary;
                response.Success = true;
                response.SuccessMessage = summary.TotalCourses == 0
                    ? $"User '{summary.FullName}' is not enrolled in any courses yet."
                    : $"Retrieved {summary.TotalCourses} course(s) for user '{summary.FullName}'.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"An unexpected error occurred while retrieving enrollments for UserId '{userId}': {ex.Message}");
            }

            return response;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET BY COURSE  →  course data once + list of students
        // ─────────────────────────────────────────────────────────────────────
        public async Task<CourseEnrollmentsResponseDto> GetEnrollmentsByCourseIdAsync(int courseId)
        {
            var response = new CourseEnrollmentsResponseDto();

            // ── Validation ──────────────────────────────────────────────────
            if (courseId <= 0)
            {
                response.Success = false;
                response.Errors.Add("CourseId must be a positive integer greater than zero.");
                return response;
            }

            try
            {
                // Verify course exists before querying enrollments
                var courseExists = await _enrollmentRepository.CourseExistsAsync(courseId);
                if (!courseExists)
                {
                    response.Success = false;
                    response.Errors.Add($"No course found with Id '{courseId}'. Please provide a valid CourseId.");
                    return response;
                }

                // Students only (teachers excluded at repo level)
                var enrollments = await _enrollmentRepository.GetByCourseIdAsync(courseId);

                // Grab course info from first enrollment (or fall back to id-only)
                var firstCourse = enrollments.FirstOrDefault()?.Course;

                var summary = new CourseEnrollmentsSummaryDto
                {
                    CourseId    = courseId,
                    CourseTitle = firstCourse?.Title    ?? $"Course {courseId}",
                    CourseCode  = firstCourse?.CourseCode ?? string.Empty,
                    Students    = enrollments
                        .Where(uc => uc.User != null)
                        .Select(uc => new EnrolledStudentDto
                        {
                            UserId      = uc.UserId,
                            FullName    = uc.User!.FullName ?? uc.User.UserName,
                            AccountType = uc.User!.accountType.ToString()
                        }).ToList()
                };
                summary.TotalStudents = summary.Students.Count;

                response.CourseEnrollments = summary;
                response.Success = true;
                response.SuccessMessage = summary.TotalStudents == 0
                    ? $"Course '{summary.CourseTitle}' has no enrolled students yet."
                    : $"Retrieved {summary.TotalStudents} student(s) enrolled in '{summary.CourseTitle}'.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"An unexpected error occurred while retrieving enrollments for CourseId '{courseId}': {ex.Message}");
            }

            return response;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET SINGLE ENROLLMENT
        // ─────────────────────────────────────────────────────────────────────
        public async Task<DataResponseDto<EnrollmentResponseItemDto>> GetEnrollmentByIdAsync(int userId, int courseId)
        {
            var response = new DataResponseDto<EnrollmentResponseItemDto>();

            // ── Validation ──────────────────────────────────────────────────
            var errors = new List<string>();

            if (userId <= 0)
                errors.Add("UserId must be a positive integer greater than zero.");

            if (courseId <= 0)
                errors.Add("CourseId must be a positive integer greater than zero.");

            if (errors.Any())
            {
                response.Success = false;
                response.Errors = errors;
                return response;
            }

            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(userId, courseId);

                if (enrollment is null)
                {
                    response.Success = false;
                    response.Errors.Add($"No enrollment found for UserId '{userId}' and CourseId '{courseId}'. " +
                                        "The user may not be enrolled in this course.");
                    return response;
                }

                var mapped = _mapper.Map<EnrollmentResponseItemDto>(enrollment);
                response.Data.Add(mapped);
                response.Success = true;
                response.SuccessMessage = $"Enrollment record retrieved successfully for UserId '{userId}' and CourseId '{courseId}'.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"An unexpected error occurred while retrieving the enrollment: {ex.Message}");
            }

            return response;
        }

        // ─────────────────────────────────────────────────────────────────────
        // ENROLL USER
        // ─────────────────────────────────────────────────────────────────────
        public async Task<EnrollmentResponseDto> EnrollUserAsync(EnrollmentDto dto)
        {
            var response = new EnrollmentResponseDto();

            // ── Validation ──────────────────────────────────────────────────
            var errors = new List<string>();

            if (dto is null)
            {
                response.Success = false;
                response.Errors.Add("Enrollment request body cannot be null.");
                return response;
            }

            if (dto.UserId <= 0)
                errors.Add("UserId must be a positive integer greater than zero.");

            if (dto.CourseId <= 0)
                errors.Add("CourseId must be a positive integer greater than zero.");

            if (errors.Any())
            {
                response.Success = false;
                response.Errors = errors;
                return response;
            }

            try
            {
                // ── Business rule: user must exist ───────────────────────────
                var user = await _userRepository.GetUserById(dto.UserId);
                if (user is null)
                {
                    response.Success = false;
                    response.Errors.Add($"Cannot enroll: no user found with Id '{dto.UserId}'. " +
                                        "Please provide a valid UserId.");
                    return response;
                }

                // ── Business rule: course must exist ─────────────────────────
                // We resolve the course through the CourseRepository using a
                // temporary UserCourse stub (same pattern as CourseService).
                var tempUc = new UserCourse { UserId = dto.UserId, CourseId = dto.CourseId };
                var course = await _courseRepository.GetByIdAsync(tempUc);

                // If no direct user↔course link exists yet, fall back to checking
                // all courses by courseId to confirm the course itself exists.
                bool courseExists = course is not null
                    || (await _enrollmentRepository.GetByCourseIdAsync(dto.CourseId)).Any()
                    || await CourseExistsAsync(dto.CourseId);

                if (!courseExists)
                {
                    response.Success = false;
                    response.Errors.Add($"Cannot enroll: no course found with Id '{dto.CourseId}'. " +
                                        "Please provide a valid CourseId.");
                    return response;
                }

                // ── Business rule: prevent duplicate enrollment ───────────────
                var alreadyEnrolled = await _enrollmentRepository.IsEnrolledAsync(dto.UserId, dto.CourseId);
                if (alreadyEnrolled)
                {
                    response.Success = false;
                    response.Errors.Add($"User '{user.FullName ?? user.UserName}' (Id: {dto.UserId}) is already " +
                                        $"enrolled in course Id '{dto.CourseId}'. Duplicate enrollments are not allowed.");
                    return response;
                }

                // ── Persist ──────────────────────────────────────────────────
                var userCourse = _mapper.Map<UserCourse>(dto);
                await _enrollmentRepository.AddAsync(userCourse);
                await _enrollmentRepository.SaveChangesAsync();

                // Return the newly created enrollment with full details
                var created = await _enrollmentRepository.GetByIdAsync(dto.UserId, dto.CourseId);
                var mapped = _mapper.Map<EnrollmentResponseItemDto>(created);

                response.Enrollments!.Add(mapped);
                response.TotalEnrollments = 1;
                response.Success = true;
                response.SuccessMessage = $"User '{user.FullName ?? user.UserName}' was successfully enrolled in " +
                                          $"course '{mapped.CourseTitle ?? dto.CourseId.ToString()}'.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"An unexpected error occurred while processing the enrollment: {ex.Message}");
            }

            return response;
        }

        // ─────────────────────────────────────────────────────────────────────
        // UNENROLL USER
        // ─────────────────────────────────────────────────────────────────────
        public async Task<EnrollmentResponseDto> UnenrollUserAsync(int userId, int courseId)
        {
            var response = new EnrollmentResponseDto();

            // ── Validation ──────────────────────────────────────────────────
            var errors = new List<string>();

            if (userId <= 0)
                errors.Add("UserId must be a positive integer greater than zero.");

            if (courseId <= 0)
                errors.Add("CourseId must be a positive integer greater than zero.");

            if (errors.Any())
            {
                response.Success = false;
                response.Errors = errors;
                return response;
            }

            try
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(userId, courseId);

                if (enrollment is null)
                {
                    response.Success = false;
                    response.Errors.Add($"Cannot unenroll: no enrollment record found for UserId '{userId}' " +
                                        $"and CourseId '{courseId}'. The user may not be enrolled in this course.");
                    return response;
                }

                // Capture names before deletion for the success message
                string userName = enrollment.User?.FullName ?? enrollment.User?.UserName ?? $"UserId {userId}";
                string courseName = enrollment.Course?.Title ?? $"CourseId {courseId}";

                await _enrollmentRepository.DeleteAsync(enrollment);
                await _enrollmentRepository.SaveChangesAsync();

                response.Success = true;
                response.SuccessMessage = $"User '{userName}' has been successfully unenrolled from course '{courseName}'.";
                response.TotalEnrollments = 0;
                response.Enrollments = new List<EnrollmentResponseItemDto>();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"An unexpected error occurred while processing the unenrollment: {ex.Message}");
            }

            return response;
        }

        // ─────────────────────────────────────────────────────────────────────
        // IS ENROLLED
        // ─────────────────────────────────────────────────────────────────────
        public async Task<bool> IsUserEnrolledAsync(int userId, int courseId)
        {
            if (userId <= 0 || courseId <= 0)
                return false;

            return await _enrollmentRepository.IsEnrolledAsync(userId, courseId);
        }

        // ─────────────────────────────────────────────────────────────────────
        // ENROLLMENT COUNT
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Returns the student count for a course, or -1 if the course does not exist.
        /// </summary>
        public async Task<int> GetEnrollmentCountByCourseIdAsync(int courseId)
        {
            if (courseId <= 0)
                return -1;

            var exists = await _enrollmentRepository.CourseExistsAsync(courseId);
            if (!exists)
                return -1; // sentinel: course not found

            return await _enrollmentRepository.GetEnrollmentCountByCourseIdAsync(courseId);
        }

        // ─────────────────────────────────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Checks whether a course with the given id has any enrollments.
        /// Used as a fallback existence check when the caller's userId has no link yet.
        /// </summary>
        private async Task<bool> CourseExistsAsync(int courseId)
        {
            return (await _enrollmentRepository.GetByCourseIdAsync(courseId)).Any();
        }

        /// <summary>Returns the current caller's userId from the JWT, or null if unavailable.</summary>
        private int? GetCurrentUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User
                            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }

        /// <summary>Returns true when the current caller's role claim is "Teacher".</summary>
        private bool IsCurrentUserTeacher()
        {
            return _httpContextAccessor.HttpContext?.User
                       .IsInRole(AccountType.Teacher.ToString()) ?? false;
        }
    }
}
