using EduTechSpace.Models;

namespace EduTechSpace.Dto
{
	public class AdminDto
	{
		public List<ApplicationUser>?users { get; set; }
		public bool Success { get; set; }
		public string? Message { get; set; }
		public List<string>? Errors { get; set; }
	}
}
