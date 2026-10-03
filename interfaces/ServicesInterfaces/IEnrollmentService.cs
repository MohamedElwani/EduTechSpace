using EduTechSpace.Dto;

namespace EduTechSpace.interfaces.ServicesInterfaces
{
    public interface IEnrollmentService
    {
        /// <summary>
        /// Retrieve all enrollments in the system.
        /// </summary>
        Task<EnrollmentResponseDto> GetAllEnrollmentsAsync();

        /// <summary>
        /// Retrieve all courses a specific user is enrolled in.
        /// User identity appears once; courses are a list.
        /// </summary>
        Task<UserEnrollmentsResponseDto> GetEnrollmentsByUserIdAsync(int userId);

        /// <summary>
        /// Retrieve all students enrolled in a specific course (teachers excluded).
        /// Course identity appears once; students are a list.
        /// </summary>
        Task<CourseEnrollmentsResponseDto> GetEnrollmentsByCourseIdAsync(int courseId);

        /// <summary>
        /// Retrieve a single enrollment record by composite key.
        /// </summary>
        Task<DataResponseDto<EnrollmentResponseItemDto>> GetEnrollmentByIdAsync(int userId, int courseId);

        /// <summary>
        /// Enroll a user in a course. Returns error if already enrolled or ids are invalid.
        /// </summary>
        Task<EnrollmentResponseDto> EnrollUserAsync(EnrollmentDto dto);

        /// <summary>
        /// Remove a user's enrollment from a course.
        /// </summary>
        Task<EnrollmentResponseDto> UnenrollUserAsync(int userId, int courseId);

        /// <summary>
        /// Check whether a specific user is enrolled in a specific course.
        /// </summary>
        Task<bool> IsUserEnrolledAsync(int userId, int courseId);

        /// <summary>
        /// Get the total number of students enrolled in a course.
        /// Returns -1 when the course does not exist.
        /// </summary>
        Task<int> GetEnrollmentCountByCourseIdAsync(int courseId);
    }
}
