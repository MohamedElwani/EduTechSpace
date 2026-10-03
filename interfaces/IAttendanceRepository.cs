using EduTechSpace.Models;

namespace EduTechSpace.interfaces
{
	public interface IAttendanceRepository
	{
		Task<List<Attendance>> GetAllAsync();

		Task<Attendance?> GetByIdAsync(int id);

		Task<List<Attendance>> GetByLectureIdAsync(int lectureId);

		Task<List<Attendance>> GetByStudentIdAsync(int studentId);

		Task<Attendance?> GetStudentAttendanceInLectureAsync(int studentId, int lectureId);

		Task AddAsync(Attendance attendance);

		Task Update(Attendance attendance);

		Task Delete(Attendance attendance);

		Task<bool> SaveChangesAsync();
	}
}
