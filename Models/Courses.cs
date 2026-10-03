using System.Text.Json.Serialization;

namespace EduTechSpace.Models
{
	public class Courses
	{
		public int Id { get; set; }

		public string CourseCode { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string? thumbnailUrl { get; set; }

		public TimeSpan CreatedAt { get; set; } = TimeSpan.Zero;

		// Navigation
		[JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
		public virtual ICollection<Lecture>? Lectures { get; set; }
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public virtual ICollection<UserCourse>? UsersCourse { get; set; }
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public virtual ICollection<Exam>? Exams { get; set; }
	}
}