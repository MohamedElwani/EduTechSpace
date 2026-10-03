using EduTechSpace.Models;

namespace EduTechSpace.Interfaces
{
    public interface IStudentExamRepository
    {
        Task<IEnumerable<StudentExam>> GetAllExamResultsByStudentIdAsync(int studentId);
        Task<IEnumerable<StudentExam>> GetAllExamResultsByInstructorIdAsync(int instructorId);
        Task<IEnumerable<StudentExam>> GetAllExamResultsByExamIdAsync(int examId);
        Task<IEnumerable<StudentExam>> GetAllExamResultsByCourseIdAsync(int courseId);

        Task AddExamResultAsync(StudentExam studentExam);
        Task UpdateExamResultAsync(StudentExam studentExam);
        Task DeleteExamResultAsync(StudentExam studentExam);
        Task SaveChangesAsync();
    }
}