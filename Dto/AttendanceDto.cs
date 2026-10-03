using EduTechSpace.Models.Enums;
using System.Text.Json.Serialization;

namespace EduTechSpace.Dto
{
	public class AttendanceDto
	{
		public int Id { get; set; }

		public DateTime AttendanceDate { get; set; }

		public TimeSpan RecordedAt { get; set; }

		public AttendanceState AttendanceState { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? Student { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? Lecture { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? Course { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? StudentId { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? LectureId { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? CourseId { get; set; }
	}
}