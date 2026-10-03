using EduTechSpace.Dto;

namespace EduTechSpace.interfaces.ServicesInterfaces
{
	public interface IAttendanceService
	{
		Task<AttendanceResponseDto> GetAllAsync();

		Task<AttendanceResponseDto> GetByLectureIdAsync(int lectureId);

		Task<AttendanceResponseDto> GetByStudentIdAsync(int studentId);

		Task<AttendanceResponseDto> GetStudentAttendanceInLectureAsync(int lectureId, int studentId);

		Task<DataResponseDto<AttendanceDto>> MarkAttendanceAsync(MarkAttendanceDto dto);

		Task<bool> UpdateStatusAsync(int attendanceId, string status);

		Task<bool> DeleteAsync(int attendanceId);

		
	}
}
