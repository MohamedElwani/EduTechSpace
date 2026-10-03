using EduTechSpace.Dto;
using EduTechSpace.Models;

namespace EduTechSpace.interfaces.ServicesInterfaces
{
	public interface ICourseService
	{
		Task<Courses> MapDtoToCourseAsync(CourseDto courseDto);
		Task<CourseResponseDto> AddCourseAsync(CourseDto courseResponseDto);
		
		Task<CourseResponseDto> GetCourseByNameAsync(string courseName);

		Task<CourseResponseDto> GetCourseByIdAsync(int courseId);
		Task<CourseResponseDto> GetAllCoursesAsync();
		Task<CourseResponseDto> UpdateCourseAsync(CourseDto courseDto);
		Task<CourseResponseDto> RemoveCourseById(int id);
		Task<string?> GetCourseNameByIdAsync(int? id);

	}
}
