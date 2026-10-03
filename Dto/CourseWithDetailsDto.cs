namespace EduTechSpace.Dto
{
	public class CourseWithDetailsDto
	{
		public int Id { get; set; }

		public string Title { get; set; } = string.Empty;

		public string? Description { get; set; }

		public string? ThumbnailUrl { get; set; }

		public List<LectureDto> Lectures { get; set; } = new();
	}
}