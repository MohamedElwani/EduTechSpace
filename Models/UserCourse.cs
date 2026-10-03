using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Models
{
	[Table("UserCourse")]
	public class UserCourse
	{
		public virtual ApplicationUser? User { get; set; }
		public virtual Courses? Course { get; set; }

		[ForeignKey("User")]
		public int UserId { get; set; }
		[ForeignKey("Course")]
		public int CourseId { get; set; }
	}
}
