using EduTechSpace.Models.Enums;

namespace EduTechSpace.Dto
{
	public class StudentWithAllDataDto
	{
		public int Id { get; set; }

		public string FullName { get; set; } = string.Empty;

		public string? NationalId { get; set; }

		public string? ImgUrl { get; set; }

		public List<Levels> Levels { get; set; } = new();

		public List<CourseWithDetailsDto> Courses { get; set; } = new();

		public List<ExamResultDto> ExamResults { get; set; } = new();

		public List<StudentAttendanceSummaryDto> Attendances { get; set; } = new();	
	}
}