namespace EduTechSpace.Dto
{
    /// <summary>
    /// User info appears exactly once; courses are a list.
    /// Returned by GET api/enrollment/user/{userId}.
    /// </summary>
    public class UserEnrollmentsSummaryDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? AccountType { get; set; }
        public int TotalCourses { get; set; }
        public List<EnrolledCourseDto> Courses { get; set; } = new();
    }
}
