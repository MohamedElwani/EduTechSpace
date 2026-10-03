using EduTechSpace.Context;
using EduTechSpace.interfaces;
using EduTechSpace.Interfaces;
using EduTechSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace EduTechSpace.Repositories
{
	public class StudentExamRepository : IStudentExamRepository
	{
		private readonly EduHubDbContext _eduHubDbContext;
		public StudentExamRepository( EduHubDbContext eduHubDbContext)
		{
			_eduHubDbContext = eduHubDbContext;
		}
		public async Task AddExamResultAsync(StudentExam studentExam)
		{
			await _eduHubDbContext.Set<StudentExam>().AddAsync(studentExam);
		}

		public async Task  DeleteExamResultAsync(StudentExam studentExam)
		{
			 _eduHubDbContext.Set<StudentExam>().Remove(studentExam);
		}

		public async Task<IEnumerable<StudentExam>> GetAllExamResultsByCourseIdAsync(int courseId)
		{
			return await _eduHubDbContext.Set<StudentExam>()
				.AsNoTracking()
				.Where(es => es.Exam.CourseId == courseId)
				.ToListAsync();
		}

		public async Task<IEnumerable<StudentExam>> GetAllExamResultsByExamIdAsync(int examId)
		{
			return await _eduHubDbContext.Set<StudentExam>()
				.AsNoTracking()
				.Where(es => es.Exam.Id == examId)
				.ToListAsync();
		}

		public async Task<IEnumerable<StudentExam>> GetAllExamResultsByInstructorIdAsync(int instructorId)
		{
		   return await _eduHubDbContext.Set<StudentExam>()
				.AsNoTracking()
				.Where(es => es.Exam.Course.UsersCourse
					.Any(uc => uc.UserId == instructorId &&
				   uc.User.accountType == AccountType.Teacher))
				.OrderBy(es =>es.StudentDegree)
				.ToListAsync();
		}

		public async Task<IEnumerable<StudentExam>> GetAllExamResultsByStudentIdAsync(int studentId)
		{
			return await _eduHubDbContext.Set<StudentExam>()
				.AsNoTracking()
				.Where(es =>es.StudentId == studentId)
				.OrderBy(es =>es.StudentDegree)
				.ToArrayAsync();
		}

		public async Task SaveChangesAsync()
		{
			await _eduHubDbContext.SaveChangesAsync();
		}

		public async Task UpdateExamResultAsync(StudentExam studentExam)
		{
			 _eduHubDbContext.Set<StudentExam>().Update(studentExam);
		}
	}
}
