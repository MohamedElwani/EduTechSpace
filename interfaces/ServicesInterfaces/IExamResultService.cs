using EduTechSpace.Dto;

namespace EduTechSpace.interfaces.ServicesInterfaces
{
	public interface IExamResultService
	{
		//Task<IEnumerable<ExamResultDto>> GetAllExamResultsAsync();	

		//Task<IEnumerable<ExamResultDto>> GetAllExamResultsByCourseIdAsync(int courseId);
		//Task<IEnumerable<ExamResultDto>> GetAllExamResultsByInstructorIdAsync(int instructorId);
		//Task<IEnumerable<ExamResultDto>> GetAllExamResultsByStudentIdAsync(int studentId);
		Task<IEnumerable<ExamWithExamResultsDto>> GetAllExamsWithResultsByCourseIdAsync(int courseId);
		Task<IEnumerable<ExamWithExamResultsDto>> GetAllExamsWithResultsByInstructorIdAsync(int instructorId);
		Task<IEnumerable<ExamWithExamResultsDto>> GetAllExamsWithResultsByStudentIdAsync(int StudentId);
		Task<DataResponseDto<ExamResultDto>> AddExamResult(AddExamResultDto examResultDto);
		Task<DataResponseDto<ExamDto>> UpdateExamResult(ExamResultDto examResultDto);
		Task<DataResponseDto<ExamDto>> DeleteExamResult(int examId);

	}
}
