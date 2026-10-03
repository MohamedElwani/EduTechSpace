namespace EduTechSpace.Dto
{
    /// <summary>
    /// Response wrapper for GET api/enrollment/course/{courseId}.
    /// Course identity appears once; enrolled students are a list.
    /// </summary>
    public class CourseEnrollmentsResponseDto
    {
        public CourseEnrollmentsSummaryDto? CourseEnrollments { get; set; }
        public bool Success { get; set; }
        public string? SuccessMessage { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
