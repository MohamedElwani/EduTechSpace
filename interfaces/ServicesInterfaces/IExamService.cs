using EduTechSpace.Dto;
using EduTechSpace.Models;

namespace EduTechSpace.interfaces.ServicesInterfaces
{
	public interface IExamService
	{
		Task<IEnumerable<ExamDto>> GetAllExamsAsync();
		Task<IEnumerable<ExamDto>> GetAllExamsByCourseIdAsync(int courseId);
		Task<IEnumerable<ExamDto>> GetAllExamsByInstructorIdAsync(int instructorId);
		
		Task<ExamDto?> GetExamByIdAsync(int examId);

		Task<DataResponseDto<ExamDto>> AddExamAsync(AddExamDto exam);
		Task<DataResponseDto<ExamDto>> UpdateExamAsync(ExamDto exam);

		Task<DataResponseDto<ExamDto>> DeleteExamAsync(int examId);
		Task<DataResponseDto<ExamDto>> DeleteExamWithResultsAsync(int examId);
	}
}
