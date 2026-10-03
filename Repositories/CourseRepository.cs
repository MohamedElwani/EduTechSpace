using EduTechSpace.Context;
using EduTechSpace.Dto;
using EduTechSpace.interfaces;
using EduTechSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace EduTechSpace.Repositories
{
	public class CourseRepository : ICourseRepository
	{
		private readonly EduHubDbContext _context;
		public CourseRepository(EduHubDbContext context)
		{
			this._context = context;
		}

		public async Task AddAsync(Courses course , UserCourse userCourse)
		{
			await _context.Courses.AddAsync(course);
			await _context.SaveChangesAsync();
			userCourse.CourseId =course.Id;
			await _context.UserCourses.AddAsync(userCourse);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Courses>> GetAllAsync()
		{
			return await  _context.Courses
				.ToListAsync();
		}

		public async Task<Courses> GetByIdAsync(UserCourse userCourse)
		{
			return _context.UserCourses
			.Where(uc => uc.UserId == userCourse.UserId && uc.CourseId == userCourse.CourseId)
			.Select(uc => uc.Course)
			.AsNoTracking().FirstOrDefault() as Courses;
		}

		public async Task<List<Courses>> GetByNameAsync(string name ,int UserId)
		{
			return await _context.UserCourses
				.Where(uc=> uc.UserId == UserId && uc.Course.Title.Contains(name))
				.Select(uc => uc.Course)
				.AsNoTracking()
				.ToListAsync();
		}

		public void Update(Courses course, UserCourse userCourse)
		{
			_context.Update(course);
			_context.Update(userCourse);
			_context.SaveChanges();
		}
		public void Delete(Courses course, UserCourse userCourse)
		{
			_context.Courses.Remove(course);
			_context.Remove(userCourse);
			_context.SaveChanges();
		}
		
	}
}
