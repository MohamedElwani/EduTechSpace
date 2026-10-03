using EduTechSpace.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Dto
{
	public class MarkAttendanceDto
	{
		public int StudentId { get; set; }
		public int LectureId { get; set; }
		public int CourseId { get; set; }
		public AttendanceState state { get; set; }
	}
}
