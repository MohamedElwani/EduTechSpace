using EduTechSpace.Models;

namespace EduTechSpace.Dto
{
	public class CourseResponseDto
	{
		public List<Courses>? Courses { get; set; } = new List<Courses>();
		public bool ? Success { get; set; }
		public string? SuccessMessage { get; set; }
		public List<string>? Errors { get; set; } = new List<string>();
	}
}
