namespace EduTechSpace.Dto
{
	public class ExamWithExamResultsDto
	{
		public ExamDto Exam { get; set; } = new ExamDto();
		public IEnumerable<ExamResultDto>? Results { get; set; }
	}
}
