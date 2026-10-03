using EduTechSpace.Models;
using EduTechSpace.Models.Enums;
using System.Collections.Generic;

namespace EduTechSpace.interfaces
{
	public interface IUserRepository
	{
		Task<IEnumerable<ApplicationUser>> GetAllStudents();
		Task<IEnumerable<ApplicationUser>> GetAllStudentsByInstructorId(int instructorId, Levels? userYear);
		Task<ApplicationUser?> GetUserById (int id);
		Task<ApplicationUser?> GetAllStudentDataById (int studentId);
		Task AddStudentToInstructor(int studentId, int instructorID);
	}
}
