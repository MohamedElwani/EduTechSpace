using EduTechSpace.Dto;
using EduTechSpace.interfaces;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Models;
using EduTechSpace.Repositories;

namespace EduTechSpace.Services
{
	public class LectureService : ILectureService
	{
		private readonly ILectureRepository _lectureRepository;
		private readonly ICourseService _courseService;
		public LectureService(ILectureRepository lectureRepository
			,ICourseService courseService)
		{
			this._lectureRepository = lectureRepository;
			_courseService = courseService;
		}
		public async Task<int> CreateAsync(AddLectureDto dto)
		{
			var course = await _courseService.GetCourseByIdAsync(dto.CourseId??0);
			if (course == null)
				return 0;
			var lecture = new Lecture
			{
				Title = dto.Title,
				CourseId = dto.CourseId,
			};

			await _lectureRepository.AddAsync(lecture);
			return lecture.Id;

		}

		public async Task<bool> DeleteAsync(int id)
		{
			var lecture = await _lectureRepository.GetByIdAsync(id);
			if (lecture == null)
				return false;
			try
			{
				_lectureRepository.Delete(lecture);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async  Task<List<LectureDto>> GetAllAsync()
		{
			var lectures = await _lectureRepository.GetAllAsync();
			return lectures.Select(l => new LectureDto
			{
				Id = l.Id,
				Title = l.Title,
				StartDate = l.StartDate,
				EndDate = l.EndDate,
				CourseId = l.CourseId,
			}).ToList();
		}

		public async Task<List<LectureDto>?> GetByCourseIdAsync(int id)
		{
			var lectures = await _lectureRepository.GetByCourseIdAsync(id);
			if (lectures == null) return null;
			try
			{ var courseName = await _courseService.GetCourseNameByIdAsync(lectures.First().CourseId); }
			catch { return null; }
			return lectures.Select(l => new LectureDto
			{
				Id = l.Id,
				Title = l.Title,
				StartDate = l.StartDate,
				EndDate = l.EndDate,
				CourseId = l.CourseId,
				CourseName = l.Course?.Title
			}).ToList();
		}

		public async Task<LectureDto?> GetByIdAsync(int id)
		{
			var lecture = await _lectureRepository.GetByIdAsync(id);
			if (lecture == null) return null;

			return new LectureDto
			{
				Id = lecture.Id,
				Title = lecture.Title,
				StartDate = lecture.StartDate,
				EndDate = lecture.EndDate,
				CourseId = lecture.CourseId,
				CourseName = await _courseService.GetCourseNameByIdAsync(lecture.CourseId)
			};
		}

		public async Task<bool> UpdateAsync(int id, AddLectureDto dto)
		{
			var lecture = await _lectureRepository.GetByIdAsync(id);
			if (lecture == null) return false;

			lecture.Title = dto.Title;
			lecture.CourseId = dto.CourseId;
			lecture.StartDate = dto.StartDate;
			lecture.EndDate = dto.EndDate;
			_lectureRepository.Update(lecture);
	

			return true;
		}

		public async Task<List<LectureDto>> GetByUserIdAsync(int userId)
		{
			var lectures = await _lectureRepository.GetByUserIdAsync(userId);
			if (lectures == null || !lectures.Any()) return new List<LectureDto>();

			return lectures.Select(l => new LectureDto
			{
				Id = l.Id,
				Title = l.Title,
				StartDate = l.StartDate,
				EndDate = l.EndDate,
				CourseId = l.CourseId,
				CourseName = l.Course?.Title
			}).ToList();
		}
	}
}
