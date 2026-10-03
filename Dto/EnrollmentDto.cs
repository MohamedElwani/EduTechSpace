using System.ComponentModel.DataAnnotations;

namespace EduTechSpace.Dto
{
    /// <summary>
    /// DTO used to enroll a user in a course.
    /// </summary>
    public class EnrollmentDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be a positive integer.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "CourseId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId must be a positive integer.")]
        public int CourseId { get; set; }
    }
}
