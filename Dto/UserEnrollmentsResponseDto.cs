namespace EduTechSpace.Dto
{
    /// <summary>
    /// Response wrapper for GET api/enrollment/user/{userId}.
    /// User identity appears once; enrolled courses are a list.
    /// </summary>
    public class UserEnrollmentsResponseDto
    {
        public UserEnrollmentsSummaryDto? UserEnrollments { get; set; }
        public bool Success { get; set; }
        public string? SuccessMessage { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
