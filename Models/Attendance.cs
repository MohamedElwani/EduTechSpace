using EduTechSpace.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Models
{
	public class Attendance
	{
		public int Id { get; set; }
		public DateTime AttendanceDate { get; set; } = DateTime.Now;
		public TimeSpan RecordedAt { get; set; } = TimeSpan.Zero;
		public AttendanceState AttendanceState { get; set; }
		// navigation property 
		virtual public ApplicationUser? User {  get; set; }
		virtual public Lecture? Lecture { get; set; }
		virtual public Courses? Course { get; set; }
		//Forigrn Keys
		[ForeignKey("User")]
		public int? UserId { get; set; }
		[ForeignKey("Lecture")]
		public int? LectureId { get; set; }
		[ForeignKey("Course")]
		public int? CourseId { get; set; }
	}
}
