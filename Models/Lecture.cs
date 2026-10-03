using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Models
{
	public class Lecture
	{
		public int Id { get; set; }
		public string Title { get; set; }

		public DateTime StartDate { get; set; } = DateTime.Now;
		public DateTime EndDate { get; set; } = DateTime.UtcNow.AddHours(2);

		//Navigation Properties
		virtual public Courses? Course { get; set; }
		virtual public List<Attendance>? Attendances { get; set; }

		//Forigen Key 
		[ForeignKey(nameof(Course))]
		public int? CourseId { get; set; }

	}
}
