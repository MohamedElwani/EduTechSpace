using EduTechSpace.Dto;

namespace EduTechSpace.interfaces.ServicesInterfaces
{
	public interface ILectureService
	{
		Task<List<LectureDto>> GetAllAsync();
		Task<LectureDto?> GetByIdAsync(int id);
		Task<List<LectureDto>> GetByCourseIdAsync(int courseId);
		Task<List<LectureDto>> GetByUserIdAsync(int userId);

		Task<int> CreateAsync(AddLectureDto dto);
		Task<bool> UpdateAsync(int id, AddLectureDto dto);
		Task<bool> DeleteAsync(int id);
	}
}
