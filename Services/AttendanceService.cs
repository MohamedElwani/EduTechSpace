using EduTechSpace.Dto;
using EduTechSpace.interfaces;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Models.Enums;

namespace EduTechSpace.Services
{
	public class AttendanceService : IAttendanceService
	{
		private readonly IAttendanceRepository _attendanceRepository;
		public AttendanceService(IAttendanceRepository attendanceRepository)
		{
			this._attendanceRepository = attendanceRepository;
		}
		public async Task<bool> DeleteAsync(int attendanceId)
		{
			var attendance = await _attendanceRepository.GetByIdAsync(attendanceId);
			if (attendance == null)
				return false;
			_attendanceRepository.Delete(attendance);
			return true;
		}
		private Dictionary<AttendanceState, int> GetStatus(List<AttendanceDto> attendances)
		{
			return attendances
				.GroupBy(a => a.AttendanceState)
				.ToDictionary(g => g.Key, g => g.Count());
		}
		public async Task<AttendanceResponseDto> GetAllAsync()
		{
			AttendanceResponseDto Attendances = new AttendanceResponseDto();
			var attendances = await _attendanceRepository.GetAllAsync();
			var mappedAttendance = attendances.Select(a => new AttendanceDto
			{
				Id = a.Id,
				AttendanceDate = a.AttendanceDate,
				RecordedAt = a.RecordedAt,
				AttendanceState = a.AttendanceState,

				Student = a.User?.UserName,
				Lecture = a.Lecture?.Title,
				Course = a.Lecture?.Course?.Title,

				StudentId = a.UserId,
				LectureId = a.LectureId,
				CourseId = a.Lecture?.CourseId
			}
			).ToList<AttendanceDto>();
			if (mappedAttendance != null)
			{
			Attendances.attendances = mappedAttendance;
				Attendances.statesCount = GetStatus(Attendances.attendances);
				Attendances.totalAttendances = Attendances.attendances.Count();
				Attendances.Successed = true;
				Attendances.SucessMessage = "All Atendances Loaded Successfully";
				return Attendances;
			}
			Attendances.Errors?.Add("Some thing happend or no Attendance Founded");
			Attendances.Successed = false;
			return Attendances;
		}

		public async Task<AttendanceResponseDto> GetByLectureIdAsync(int lectureId)
		{
			var response = new AttendanceResponseDto();

			var data = await _attendanceRepository.GetByLectureIdAsync(lectureId);

			var mapped = data.Select(a => new AttendanceDto
			{
				Id = a.Id,
				AttendanceDate = a.AttendanceDate,
				RecordedAt = a.RecordedAt,
				AttendanceState = a.AttendanceState,
				Student = a.User?.UserName,
				Lecture = a.Lecture?.Title,
				Course = a.Course?.Title,
				StudentId = a.UserId,
				LectureId = a.LectureId,
				CourseId = a.Lecture?.CourseId
			}).ToList();

			response.attendances = mapped;
			response.totalAttendances = mapped.Count;
			response.statesCount = GetStatus(mapped);
			response.Successed = true;

			return response;
		}

		public async Task<AttendanceResponseDto> GetByStudentIdAsync(int studentId)
		{
			var response = new AttendanceResponseDto();

			var data = await _attendanceRepository.GetByStudentIdAsync(studentId);

			var mapped = data.Select(a => new AttendanceDto
			{
				Id = a.Id,
				AttendanceDate = a.AttendanceDate,
				RecordedAt = a.RecordedAt,
				AttendanceState = a.AttendanceState,
				Student = a.User?.UserName,
				Lecture = a.Lecture?.Title,
				Course = a.Course?.Title,
				StudentId = a.UserId,
				LectureId = a.LectureId,
				CourseId = a.Lecture?.CourseId
			}).ToList();

			response.attendances = mapped;
			response.totalAttendances = mapped.Count;
			response.statesCount = GetStatus(mapped);
			response.Successed = true;

			return response;
		}

		public async Task<AttendanceResponseDto> GetStudentAttendanceInLectureAsync(int lectureId, int studentId)
		{
			var response = new AttendanceResponseDto();

			var attendance = await _attendanceRepository
				.GetStudentAttendanceInLectureAsync(studentId, lectureId);

			if (attendance == null)
			{
				response.Successed = false;
				response.Errors = new List<string> { "Attendance not found" };
				return response;
			}

			var mapped = new List<AttendanceDto>
	{
		new AttendanceDto
		{
			Id = attendance.Id,
			AttendanceDate = attendance.AttendanceDate,
			RecordedAt = attendance.RecordedAt,
			AttendanceState = attendance.AttendanceState,

			Student = attendance.User?.UserName,
			Lecture = attendance.Lecture?.Title,
			Course = attendance.Course?.Title,

			StudentId = attendance.UserId,
			LectureId = attendance.LectureId,
			CourseId = attendance.Lecture?.CourseId
		}
	};

			response.attendances = mapped;
			response.totalAttendances = 1;
			response.statesCount = GetStatus(mapped);
			response.Successed = true;

			return response;
		}

		public async Task<DataResponseDto<AttendanceDto>> MarkAttendanceAsync(MarkAttendanceDto dto)
		{
			var result = new DataResponseDto<AttendanceDto>();

			var errors = new List<string>();

			if (dto.StudentId <= 0)
				errors.Add("Invalid StudentId");

			if (dto.LectureId <= 0)
				errors.Add("Invalid LectureId");

			if (errors.Any())
			{
				result.Success = false;
				result.Errors = errors;
				return result;
			}

			var existing = await _attendanceRepository
				.GetStudentAttendanceInLectureAsync(dto.StudentId, dto.LectureId);

			if (existing != null)
			{
				result.Success = false;
				result.Errors.Add("Attendance already exists");
				return result;
			}

			var attendance = new Models.Attendance
			{
				UserId = dto.StudentId,
				LectureId = dto.LectureId,
				CourseId =  dto.CourseId,
				AttendanceDate = DateTime.Now,
				RecordedAt = DateTime.Now.TimeOfDay,
				AttendanceState = dto.state
			};

			await _attendanceRepository.AddAsync(attendance);

			result.Success = true;
			result.SuccessMessage = "Attendance marked successfully";

			result.Data.Add(new AttendanceDto
			{
				Id = attendance.Id,
				StudentId = attendance.UserId,
				LectureId = attendance.LectureId,
				AttendanceState = attendance.AttendanceState,
				AttendanceDate = attendance.AttendanceDate,
				RecordedAt = attendance.RecordedAt
			});

			return result;
		}

		public async Task<bool> UpdateStatusAsync(int attendanceId, string status)
		{
			var attendance = await _attendanceRepository.GetByIdAsync(attendanceId);

			if (attendance == null)
				return false;

			if (!Enum.TryParse(status, true, out AttendanceState newState))
				return false;

			attendance.AttendanceState = newState;

			await _attendanceRepository.Update(attendance);
			await _attendanceRepository.SaveChangesAsync();

			return true;
		}
	}
}
