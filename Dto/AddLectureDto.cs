using EduTechSpace.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Dto
{
	public class AddLectureDto
	{
		public string Title { get; set; }
		public DateTime StartDate { get; set; } = DateTime.Now;
		public DateTime EndDate { get; set; } = DateTime.UtcNow.AddHours(2);
		public int? CourseId { get; set; }
	}
}
