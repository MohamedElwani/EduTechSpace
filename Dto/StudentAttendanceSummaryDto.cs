namespace EduTechSpace.Dto
{
    public class StudentAttendanceSummaryDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int TotalLectures { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }
        public int BannedCount { get; set; }
    }
}
