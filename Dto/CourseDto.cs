using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EduTechSpace.Dto
{
	public class CourseDto
	{
		public int? Id { get; set; }
		[Required]
		public string CourseCode { get; set; }
		[JsonIgnore]
		public int?InstructorId { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public IFormFile? Thumbnail { get; set; }
        
	}
}
