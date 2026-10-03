using EduTechSpace.Models;

namespace EduTechSpace.interfaces
{
    public interface IEnrollmentRepository
    {
        /// <summary>
        /// Get all enrollments (UserCourse records).
        /// </summary>
        Task<List<UserCourse>> GetAllAsync();

        /// <summary>
        /// Get a specific enrollment by composite key (UserId + CourseId).
        /// </summary>
        Task<UserCourse?> GetByIdAsync(int userId, int courseId);

        /// <summary>
        /// Get all enrollments for a specific user.
        /// </summary>
        Task<List<UserCourse>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Get all enrollments for a specific course.
        /// </summary>
        Task<List<UserCourse>> GetByCourseIdAsync(int courseId);

        /// <summary>
        /// Check whether a user is already enrolled in a course.
        /// </summary>
        Task<bool> IsEnrolledAsync(int userId, int courseId);

        /// <summary>
        /// Enroll a user in a course.
        /// </summary>
        Task AddAsync(UserCourse userCourse);

        /// <summary>
        /// Remove a user's enrollment from a course.
        /// </summary>
        Task DeleteAsync(UserCourse userCourse);

        /// <summary>
        /// Get the count of students enrolled in a given course.
        /// </summary>
        Task<int> GetEnrollmentCountByCourseIdAsync(int courseId);

        /// <summary>
        /// Check whether a course with the given id exists in the database.
        /// </summary>
        Task<bool> CourseExistsAsync(int courseId);

        /// <summary>
        /// Persist any pending changes to the database.
        /// </summary>
        Task<bool> SaveChangesAsync();
    }
}
