namespace EduTechSpace.Dto
{
    /// <summary>
    /// A single course entry inside a user's enrollment summary.
    /// </summary>
    public class EnrolledCourseDto
    {
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseCode { get; set; }
    }
}
