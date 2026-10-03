using EduTechSpace.Context;
using EduTechSpace.interfaces;
using EduTechSpace.Models;
using EduTechSpace.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduTechSpace.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly EduHubDbContext _context;

		public UserRepository(EduHubDbContext context)
		{
			_context = context;
		}

		public async Task<ApplicationUser?> GetUserById(int id)
		{
			return await _context.Users
				.Include(u => u.UserCourses)
					.ThenInclude(uc => uc.Course)
				.Include(u => u.ExamResults)
					.ThenInclude(se => se.Exam)
				.Include(u => u.UserYears)
				.FirstOrDefaultAsync(u => u.Id == id);
		}

		public async Task<IEnumerable<ApplicationUser>> GetAllStudents()
		{
			return await _context.Users
				.Where(u => u.accountType == AccountType.Student)
				.Include(u => u.UserCourses)
					.ThenInclude(uc => uc.Course)
					.ThenInclude(c => c.Lectures)
					.ThenInclude(l => l.Attendances)
				.Include(u => u.ExamResults)
					.ThenInclude(se => se.Exam)
				.Include(u => u.UserYears)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IEnumerable<ApplicationUser>> GetAllStudentsByInstructorId(int instructorId, Levels? userYear)
		{
			var query = _context.Users
				.Where(u => u.accountType == AccountType.Student &&
							u.InstructorId == instructorId)
				.Include(u => u.UserCourses)
					.ThenInclude(uc => uc.Course)
					.ThenInclude(c => c.Lectures)
					.ThenInclude(l => l.Attendances)
				.Include(u => u.ExamResults)
					.ThenInclude(se => se.Exam)
				.Include(u => u.UserYears)
				.AsQueryable();

			if (userYear.HasValue)
			{
				query = query.Where(u =>
					u.UserYears.Any(y => y.UserLevel == userYear.Value));
			}

			return await query.AsNoTracking().ToListAsync();
		}

		public async Task<ApplicationUser?> GetAllStudentDataById(int studentId)
		{
			return await _context.Users
				.Where(u => u.Id == studentId &&
							u.accountType == AccountType.Student)
				.Include(u => u.UserCourses)
					.ThenInclude(uc => uc.Course)
						.ThenInclude(c => c.Lectures)
							.ThenInclude(l => l.Attendances)
				.Include(u => u.ExamResults)
					.ThenInclude(se => se.Exam)
				.Include(u => u.UserYears)
				.AsNoTracking()
				.FirstOrDefaultAsync();
		}

		public async Task AddStudentToInstructor(int studentId, int instructorId)
		{
			var student = await _context.Users
				.FirstOrDefaultAsync(u => u.Id == studentId);

			if (student == null || student.accountType != AccountType.Student)
				return;

			var instructor = await _context.Users
				.FirstOrDefaultAsync(u => u.Id == instructorId);

			if (instructor == null || instructor.accountType != AccountType.Teacher)
				return;

			student.InstructorId = instructorId;

			await _context.SaveChangesAsync();
		}
	}
}