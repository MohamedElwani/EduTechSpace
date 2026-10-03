namespace EduTechSpace.Dto
{
    /// <summary>
    /// A single student entry inside a course's enrollment summary.
    /// </summary>
    public class EnrolledStudentDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? AccountType { get; set; }
    }
}
