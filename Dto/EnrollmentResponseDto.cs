namespace EduTechSpace.Dto
{
    /// <summary>
    /// Wrapper response DTO for enrollment service operations. //total enrollments
    /// Follows the same pattern as CourseResponseDto / AttendanceResponseDto.
    /// </summary>
    public class EnrollmentResponseDto
    {
        public List<EnrollmentResponseItemDto>? Enrollments { get; set; } = new List<EnrollmentResponseItemDto>();
        public int? TotalEnrollments { get; set; }
        public bool Success { get; set; }
        public string? SuccessMessage { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
