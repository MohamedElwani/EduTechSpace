using System.Text.Json.Serialization;

namespace EduTechSpace.Dto
{
	public class LectureDto
	{
		public int Id { get; set; }

		public string Title { get; set; } = string.Empty;
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? CourseId { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? CourseName { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public List<AttendanceDto> Attendances { get; set; } = new();
	}
}