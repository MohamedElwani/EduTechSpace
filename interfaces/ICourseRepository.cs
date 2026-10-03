using EduTechSpace.Dto;
using EduTechSpace.Models;

namespace EduTechSpace.interfaces
{
	public interface ICourseRepository
	{
		Task<Courses> GetByIdAsync(UserCourse userCourse);
		Task<List<Courses>> GetAllAsync();
		Task AddAsync(Courses course, UserCourse userCourse);
		void Update(Courses course, UserCourse userCourse);
		void Delete(Courses course, UserCourse userCourse);

		Task<List<Courses>> GetByNameAsync(string name,int UserId);
	}
}
