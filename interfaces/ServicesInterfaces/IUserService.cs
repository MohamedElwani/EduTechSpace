using EduTechSpace.Dto;
using EduTechSpace.Models.Enums;

namespace EduTechSpace.interfaces.ServicesInterfaces
{
	public interface IUserService
	{
		Task<IEnumerable<StudentWithAllDataDto>> GetAllStudentsAsync();

		Task<IEnumerable<StudentWithAllDataDto>> GetStudentsByInstructorAsync(int instructorId, Levels? level);

		Task<StudentWithAllDataDto?> GetStudentDetailsAsync(int studentId);

		Task<IEnumerable<StudentDto>> GetStudentNamesOnlyAsync();

		Task AddStudentToInstructorAsync(int studentId, int instructorId);
	}
}