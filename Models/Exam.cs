using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Models
{
	public class Exam
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public int MaxDegree { get; set; }

		public int MinDegree { get; set; }

		// Navigation properties
		public virtual ICollection<StudentExam>? StudentExams { get; set; }

		public virtual Courses? Course { get; set; }

		// Foreign Key
		[ForeignKey("Course")]
		public int CourseId { get; set; }
	}
}