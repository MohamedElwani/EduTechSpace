using EduTechSpace.Context;
using EduTechSpace.interfaces;
using EduTechSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace EduTechSpace.Repositories
{
	public class ExamRepository : IExamRepository
	{
		private readonly EduHubDbContext _eduTechSpace;
		public ExamRepository(EduHubDbContext eduHubDbContext)
		{
			_eduTechSpace = eduHubDbContext;
		}
		public async Task AddExamAsync(Exam exam)
		{
			await _eduTechSpace.exams.AddAsync(exam);
		}

		public async Task DeleteExamAsync(Exam exam)
		{
			_eduTechSpace.exams.Remove(exam);
		}

		public async Task<IEnumerable<Exam>> GetAllExamsAsync()
		{
			return await _eduTechSpace.exams.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IEnumerable<Exam>>? GetAllExamsByCourseIdAsync(int courseId)
		{
			return await _eduTechSpace.exams
				.AsNoTracking()
				.Where(e=>e.CourseId == courseId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Exam>>? GetAllExamsByInstructorIdAsync(int InstructorId)
		{
			return await _eduTechSpace.exams
			.AsNoTracking()
			.Where(e => e.Course.UsersCourse
		    .Any(uc => uc.UserId == InstructorId))
	        .ToListAsync();
		}

		public async Task<Exam?> GetExamByIdAsync(int Id)
		{
			return await _eduTechSpace.exams.AsNoTracking().FirstOrDefaultAsync(e=>e.Id == Id);
		}

		public async Task SaveChangesAsync()
		{
			await _eduTechSpace.SaveChangesAsync();
		}

		public async Task UpdateExamAsync(Exam exam)
		{
			_eduTechSpace.exams.Update(exam);
		}
	}
}
