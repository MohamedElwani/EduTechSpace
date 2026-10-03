using EduTechSpace.Context;
using EduTechSpace.interfaces;
using EduTechSpace.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EduTechSpace.Repositories
{
	public class AttendanceRepository : IAttendanceRepository
	{
		private readonly EduHubDbContext _eduHubDbContext;
		public AttendanceRepository(EduHubDbContext eduHubDbContext)
		{
			this._eduHubDbContext = eduHubDbContext;
		}
		public async Task AddAsync(Attendance attendance)
		{
			await _eduHubDbContext.Attendances.AddAsync(attendance);
			await SaveChangesAsync();
		}

		public async Task Delete(Attendance attendance)
		{
			_eduHubDbContext.Attendances.Remove(attendance);
			await SaveChangesAsync();
		}

		public async  Task<List<Attendance>> GetAllAsync()
		{
			return await this._eduHubDbContext.Attendances
				.Include(At=>At.Lecture)
				.Include(At =>At.Course)
				.Include(At=>At.User)
				.ToListAsync();
		}

		public async Task<Attendance?> GetByIdAsync(int id)
		{
			return await this._eduHubDbContext.Attendances
				.Where(a => a.Id ==id)
				.Include(At => At.Lecture)
				.Include(At => At.Course)
				.Include(At => At.User)
				.AsNoTracking()
				.FirstOrDefaultAsync();
		}

		public async Task<List<Attendance>> GetByLectureIdAsync(int lectureId)
		{
			return await this._eduHubDbContext.Attendances
				.Where(a => a.LectureId == lectureId)
				.Include(At => At.Lecture)
				.Include(At => At.Course)
				.Include(At => At.User)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<List<Attendance>> GetByStudentIdAsync(int studentId)
		{
			return await this._eduHubDbContext.Attendances
				.Where(a => a.UserId == studentId)
				.Include(At => At.Lecture)
				.Include(At => At.Course)
				.Include(At => At.User)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<Attendance?> GetStudentAttendanceInLectureAsync(int studentId, int lectureId)
		{
			return await this._eduHubDbContext.Attendances
				.Where(a => a.UserId == studentId && a.LectureId == lectureId)
				.Include(At => At.Lecture)
				.Include(At => At.Course)
				.Include(At => At.User)
				.AsNoTracking()
				.FirstOrDefaultAsync();
		}

		public async Task<bool> SaveChangesAsync()
		{
			return (await _eduHubDbContext.SaveChangesAsync()>0)? true : false;
		}

		public async Task Update(Attendance attendance)
		{
			 _eduHubDbContext.Attendances.Update(attendance);
			await SaveChangesAsync();
		}

	}
}
