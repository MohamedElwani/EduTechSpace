using EduTechSpace.Context;
using EduTechSpace.interfaces;
using EduTechSpace.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EduTechSpace.Repositories
{
	public class LectureRepository : ILectureRepository
	{
		private readonly EduHubDbContext _eduHubDb;
		public LectureRepository(EduHubDbContext eduHubDb)
		{
		   this._eduHubDb = eduHubDb;	
		}
		public async Task AddAsync(Lecture lecture)
		{
			 await this._eduHubDb.lectures.AddAsync(lecture);
			this._eduHubDb.SaveChanges();
		}

		public async void Delete(Lecture lecture)
		{
			 this._eduHubDb.lectures.Remove(lecture);
			this._eduHubDb.SaveChanges();
		}

		public async Task<List<Lecture>> GetAllAsync()
		{
			return await this._eduHubDb.lectures.ToListAsync();
		}

		public async Task<List<Lecture>> GetByCourseIdAsync(int courseId)
		{
			return await this._eduHubDb.lectures.Where(L=>L.CourseId == courseId).ToListAsync();
		}

		public async Task<List<Lecture>> GetByUserIdAsync(int userId)
		{
			return await this._eduHubDb.lectures
				.Where(l => l.Course.UsersCourse
					.Any(uc => uc.UserId == userId))
				.ToListAsync();
		}


		public async Task<Lecture?> GetByIdAsync(int id)
		{
			return  this._eduHubDb.lectures.FirstOrDefault(L=>L.Id == id);
		}

		public async void SaveChangesAsync()
		{
			 this._eduHubDb.SaveChanges();
		}

		public void Update(Lecture lecture)
		{
			this._eduHubDb.lectures.Update(lecture);
			SaveChangesAsync();
		}
	}
}
