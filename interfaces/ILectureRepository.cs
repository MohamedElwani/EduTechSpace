using EduTechSpace.Models;

namespace EduTechSpace.interfaces
{
	public interface ILectureRepository
	{
		Task<List<Lecture>> GetAllAsync();
		Task<Lecture?> GetByIdAsync(int id);

		Task<List<Lecture>> GetByCourseIdAsync(int courseId);

		Task<List<Lecture>> GetByUserIdAsync(int userId);

		Task AddAsync(Lecture lecture);
		void Update(Lecture lecture);
		void Delete(Lecture lecture);

		void SaveChangesAsync();
	}
}
