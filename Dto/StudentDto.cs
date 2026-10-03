using EduTechSpace.Models.Enums;

namespace EduTechSpace.Dto
{
	public class StudentDto
	{
		public int Id { get; set; }

		public string FullName { get; set; } = string.Empty;

		public string? NationalId { get; set; }

		public string? ImgUrl { get; set; }

		public List<Levels> Levels { get; set; } = new();
	}
}