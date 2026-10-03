namespace EduTechSpace.Dto
{
    /// <summary>
    /// Lightweight read DTO representing a single enrollment record.
    /// </summary>
    public class EnrollmentResponseItemDto
    {
        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        /// <summary>Student or Teacher</summary>
        public string? AccountType { get; set; }
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseCode { get; set; }
    }
}

