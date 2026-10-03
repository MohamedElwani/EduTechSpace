using EduTechSpace.Models.Enums;

namespace EduTechSpace.Dto
{
	public class AttendanceResponseDto
	{
		public List<AttendanceDto>? attendances { get; set; }
		public int? totalAttendances { get; set; }
		public Dictionary<AttendanceState, int>? statesCount { get; set; } = new Dictionary<AttendanceState, int>();
		public List<string>? Errors { get; set; } = new List<string>();
		public bool Successed { get; set; }
		public string? SucessMessage { get; set; }
	}
}
