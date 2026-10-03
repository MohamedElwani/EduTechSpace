using EduTechSpace.Models;

namespace EduTechSpace.interfaces
{
	public interface IExamRepository
	{
		Task<IEnumerable<Exam>> GetAllExamsAsync();
		Task<IEnumerable<Exam>>? GetAllExamsByCourseIdAsync(int courseId);
		Task<IEnumerable<Exam>>? GetAllExamsByInstructorIdAsync(int InstructorId);
		Task<Exam?> GetExamByIdAsync(int Id);
		Task AddExamAsync(Exam exam);
		Task UpdateExamAsync(Exam exam);

		Task DeleteExamAsync(Exam exam);
		Task SaveChangesAsync();
	}
}
