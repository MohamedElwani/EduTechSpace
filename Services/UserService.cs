using AutoMapper;
using EduTechSpace.Dto;
using EduTechSpace.interfaces;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Models;
using EduTechSpace.Models.Enums;

namespace EduTechSpace.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMapper _mapper;

		public UserService(IUserRepository userRepository
			, IMapper mapper
			,IWebHostEnvironment webHostEnvironment,
			IHttpContextAccessor httpContextAccessor)
		{
			_userRepository = userRepository;
			_mapper = mapper;
			_webHostEnvironment = webHostEnvironment;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<IEnumerable<StudentWithAllDataDto>> GetAllStudentsAsync()
		{
			var students = await _userRepository.GetAllStudents();

			if (students == null || !students.Any())
				return Enumerable.Empty<StudentWithAllDataDto>();
			string CurrentPath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}"; ;
			foreach (var student in students)
			{
				student.imgUrl = $"{CurrentPath}/{student.imgUrl}";
			}

			return _mapper.Map<IEnumerable<StudentWithAllDataDto>>(students);
		}

		public async Task<IEnumerable<StudentWithAllDataDto>> GetStudentsByInstructorAsync(int instructorId, Levels? level)
		{
			if (instructorId <= 0)
				throw new ArgumentException("Invalid instructor id");

			var students = await _userRepository.GetAllStudentsByInstructorId(instructorId, level);
			string CurrentPath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}"; ;
			foreach (var student in students)
			{
				student.imgUrl = $"{CurrentPath}/{student.imgUrl}";
			}

			if (students == null || !students.Any())
				return Enumerable.Empty<StudentWithAllDataDto>();

			return _mapper.Map<IEnumerable<StudentWithAllDataDto>>(students);
		}

		public async Task<StudentWithAllDataDto?> GetStudentDetailsAsync(int studentId)
		{
			if (studentId <= 0)
				throw new ArgumentException("Invalid student id");

			var student = await _userRepository.GetAllStudentDataById(studentId);

			if (student == null)
				return null;
			string CurrentPath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}"; ;
			student.imgUrl = $"{CurrentPath}/{student.imgUrl}";
			
			return _mapper.Map<StudentWithAllDataDto>(student);
		}

		public async Task<IEnumerable<StudentDto>> GetStudentNamesOnlyAsync()
		{
			var students = await _userRepository.GetAllStudents();

			if (students == null || !students.Any())
				return Enumerable.Empty<StudentDto>();

			return students.Select(s => new StudentDto
			{
				Id = s.Id,
				FullName = s.FullName ?? string.Empty
			});
		}

		public async Task AddStudentToInstructorAsync(int studentId, int instructorId)
		{
			if (studentId <= 0)
				throw new ArgumentException("Invalid student id");

			if (instructorId <= 0)
				throw new ArgumentException("Invalid instructor id");

			var student = await _userRepository.GetUserById(studentId);

			if (student == null)
				throw new KeyNotFoundException("Student not found");

			if (student.accountType != AccountType.Student)
				throw new InvalidOperationException("User is not a student");

			var instructor = await _userRepository.GetUserById(instructorId);

			if (instructor == null)
				throw new KeyNotFoundException("Instructor not found");

			if (instructor.accountType != AccountType.Teacher)
				throw new InvalidOperationException("User is not an instructor");

			await _userRepository.AddStudentToInstructor(studentId, instructorId);
		}
	}
}