namespace EduTechSpace.Dto
{
    /// <summary>
    /// Course info appears exactly once; enrolled students are a list.
    /// Returned by GET api/enrollment/course/{courseId}.
    /// </summary>
    public class CourseEnrollmentsSummaryDto
    {
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseCode { get; set; }
        public int TotalStudents { get; set; }
        public List<EnrolledStudentDto> Students { get; set; } = new();
    }
}
