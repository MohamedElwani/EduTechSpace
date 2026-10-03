using AutoMapper;
using EduTechSpace.Dto;
using EduTechSpace.Models;
using EduTechSpace.Models.Enums;

namespace EduTechSpace.Mapping
{
	public class MappingProfiler : Profile
	{
		public MappingProfiler()
		{

			CreateMap<ApplicationUser, StudentDto>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
				.ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.imgUrl));

			CreateMap<ApplicationUser, StudentWithAllDataDto>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
				.ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))
				.ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.imgUrl))
				

				.ForMember(dest => dest.Levels, opt => opt.MapFrom(src =>
					src.UserYears != null ? src.UserYears.Select(y => y.UserLevel) : new List<Levels>()))

				.ForMember(dest => dest.Courses, opt => opt.MapFrom(src =>
					src.UserCourses != null ? src.UserCourses.Where(uc => uc.Course != null).Select(uc => new CourseWithDetailsDto
					{
						Id = uc.Course.Id,
						Title = uc.Course.Title ?? string.Empty,
						Description = uc.Course.Description,
						ThumbnailUrl = uc.Course.thumbnailUrl,
						Lectures = uc.Course.Lectures != null 
							? uc.Course.Lectures
								.Where(l => l.Attendances != null && l.Attendances.Any(a => a.UserId == src.Id))
								.Select(l => new LectureDto
								{
									Id = l.Id,
									Title = l.Title,
									CourseId = null,
									CourseName = null,
									StartDate = l.StartDate,
									EndDate = l.EndDate,
									Attendances = l.Attendances.Where(a => a.UserId == src.Id).Select(a => new AttendanceDto
									{
										Id = a.Id,
										AttendanceDate = a.AttendanceDate,
										RecordedAt = a.RecordedAt,
										AttendanceState = a.AttendanceState,
										Student = null,
										Lecture = null,
										Course = null,
										StudentId = null,
										LectureId = null,
										CourseId = null
									}).ToList()
								}).ToList()
							: new List<LectureDto>()
					}) : new List<CourseWithDetailsDto>()))

				.ForMember(dest => dest.ExamResults, opt => opt.MapFrom(src => src.ExamResults))

				.ForMember(dest => dest.Attendances, opt => opt.MapFrom(src =>
					src.UserCourses != null
						? src.UserCourses
							.Where(uc => uc.Course != null && uc.Course.Lectures != null)
							.Select(uc => new StudentAttendanceSummaryDto
							{
								CourseId = uc.CourseId,
								CourseTitle = uc.Course.Title ?? string.Empty,
								TotalLectures = uc.Course.Lectures.Count,
								PresentCount = uc.Course.Lectures.SelectMany(l => l.Attendances ?? new List<Attendance>()).Count(a => a.UserId == src.Id && a.AttendanceState == AttendanceState.Present),
								AbsentCount = uc.Course.Lectures.SelectMany(l => l.Attendances ?? new List<Attendance>()).Count(a => a.UserId == src.Id && a.AttendanceState == AttendanceState.Absent),
								LateCount = uc.Course.Lectures.SelectMany(l => l.Attendances ?? new List<Attendance>()).Count(a => a.UserId == src.Id && a.AttendanceState == AttendanceState.Late),
								ExcusedCount = uc.Course.Lectures.SelectMany(l => l.Attendances ?? new List<Attendance>()).Count(a => a.UserId == src.Id && a.AttendanceState == AttendanceState.Excused),
								BannedCount = uc.Course.Lectures.SelectMany(l => l.Attendances ?? new List<Attendance>()).Count(a => a.UserId == src.Id && a.AttendanceState == AttendanceState.Banned),
							}).ToList()
						: new List<StudentAttendanceSummaryDto>()
				));

			
			CreateMap<Courses, CourseWithDetailsDto>();

			CreateMap<StudentExam, ExamResultDto>()
				.ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : "No Name"))
				.ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.StudentDegree))
				.ForMember(dest => dest.MaxDegree, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.MaxDegree : 0));
			CreateMap<Exam, ExamDto>();
			CreateMap<ExamDto, Exam>();
			CreateMap<AddExamDto, Exam>();
			CreateMap<ExamResultDto, StudentExam>()
	.ForMember(dest => dest.StudentDegree, opt => opt.MapFrom(src => src.Degree))
	.ForPath(dest => dest.Exam, opt => opt.Ignore());
			CreateMap<AddExamResultDto, StudentExam>()
	.ForMember(dest => dest.StudentDegree, opt => opt.MapFrom(src => src.Degree))
	.ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.examId))
	.ForMember(dest => dest.Student, opt => opt.Ignore())
	.ForMember(dest => dest.Exam, opt => opt.Ignore());
			CreateMap<Attendance, AttendanceDto>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.AttendanceDate, opt => opt.MapFrom(src => src.AttendanceDate))
				.ForMember(dest => dest.RecordedAt, opt => opt.MapFrom(src => src.RecordedAt))
				.ForMember(dest => dest.AttendanceState, opt => opt.MapFrom(src => src.AttendanceState))
				.ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : ""))
				.ForMember(dest => dest.Lecture, opt => opt.MapFrom(src => src.Lecture != null ? src.Lecture.Title : ""))
				.ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : ""));
			// Enrollment (UserCourse) mappings
			CreateMap<UserCourse, EnrollmentResponseItemDto>()
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
				.ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
				.ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.User != null ? src.User.accountType.ToString() : string.Empty))
				.ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
				.ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : string.Empty))
				.ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course != null ? src.Course.CourseCode : string.Empty));

			CreateMap<EnrollmentDto, UserCourse>()
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
				.ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
				.ForMember(dest => dest.User, opt => opt.Ignore())
				.ForMember(dest => dest.Course, opt => opt.Ignore());
		}
	}
}