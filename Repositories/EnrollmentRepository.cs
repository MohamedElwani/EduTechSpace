using EduTechSpace.Context;
using EduTechSpace.interfaces;
using EduTechSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace EduTechSpace.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly EduHubDbContext _context;

        public EnrollmentRepository(EduHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserCourse>> GetAllAsync()
        {
            return await _context.UserCourses
                .Include(uc => uc.User)
                .Include(uc => uc.Course)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserCourse?> GetByIdAsync(int userId, int courseId)
        {
            return await _context.UserCourses
                .Include(uc => uc.User)
                .Include(uc => uc.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
        }

        public async Task<List<UserCourse>> GetByUserIdAsync(int userId)
        {
            return await _context.UserCourses
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.User)   // needed for UserFullName mapping
                .Include(uc => uc.Course) // needed for CourseTitle/CourseCode mapping
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<UserCourse>> GetByCourseIdAsync(int courseId)
        {
            return await _context.UserCourses
                .Where(uc => uc.CourseId == courseId
                          && uc.User.accountType == Models.AccountType.Student) // exclude Teacher owners
                .Include(uc => uc.User)   // needed for UserFullName mapping
                .Include(uc => uc.Course) // needed for CourseTitle/CourseCode mapping
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> IsEnrolledAsync(int userId, int courseId)
        {
            return await _context.UserCourses
                .AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
        }

        public async Task AddAsync(UserCourse userCourse)
        {
            await _context.UserCourses.AddAsync(userCourse);
        }

        public async Task DeleteAsync(UserCourse userCourse)
        {
            _context.UserCourses.Remove(userCourse);
            await Task.CompletedTask;
        }

        public async Task<int> GetEnrollmentCountByCourseIdAsync(int courseId)
        {
            return await _context.UserCourses
                .CountAsync(uc => uc.CourseId == courseId
                               && uc.User.accountType == Models.AccountType.Student);
        }

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _context.Courses.AnyAsync(c => c.Id == courseId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
