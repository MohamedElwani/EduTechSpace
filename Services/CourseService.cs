using EduTechSpace.Context;
using EduTechSpace.Dto;
using EduTechSpace.interfaces;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduTechSpace.Services
{
	public class CourseService : ICourseService
	{
		private readonly ICourseRepository _courseRepository;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly EduHubDbContext _hubDbContext;
		private readonly UserManager<ApplicationUser> _userManager;

		public CourseService(ICourseRepository courseRepository
			,IHttpContextAccessor httpContextAccessor
			,IWebHostEnvironment hostEnvironment
			,EduHubDbContext eduHubDbContext
			,UserManager<ApplicationUser> userManager)
		{
			this._courseRepository = courseRepository;
			this._httpContextAccessor = httpContextAccessor;
			this._hostEnvironment = hostEnvironment;
			this._hubDbContext = eduHubDbContext;
			this._userManager = userManager;
		}
		public async Task<Courses> MapDtoToCourseAsync(CourseDto courseDto)
		{
			Courses course =new Courses();
			course.Title = courseDto.Title;
			course.Description = courseDto.Description;
			course.CourseCode = courseDto.CourseCode;
	        
			return course;
		}
		public async Task<CourseResponseDto> AddCourseAsync(CourseDto courseDto)
		{
			CourseResponseDto responseDto = new CourseResponseDto();
			var course = await MapDtoToCourseAsync(courseDto);
			int? instructorId = int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
			UserCourse userCourse = new()
;			if (course != null)
			{
				userCourse.CourseId = course.Id;
				userCourse.UserId = instructorId??0;
				if (userCourse?.UserId != null )
				{
					if(courseDto.Thumbnail != null)
					{
						string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(courseDto.Thumbnail.FileName);
						string FullPath = Path.Combine(_hostEnvironment.WebRootPath, "images", ImageName);
						using (var Stream = new FileStream(FullPath, FileMode.Create))
						{
							await courseDto.Thumbnail.CopyToAsync(Stream);
						}
						course.thumbnailUrl = $"images/{ImageName}";
					}
					await _courseRepository.AddAsync(course,userCourse);
					responseDto.Success = true;
					responseDto.SuccessMessage = $"Course {course.Title} Saved To DataBase ";
					return responseDto;
				}
			}
			responseDto.Success = false;
			responseDto.Errors?.Add("Error accured while saving the course");
			return responseDto;
		}

		public async Task<CourseResponseDto> GetAllCoursesAsync()
		{
			CourseResponseDto responseDto = new CourseResponseDto();
			int instructorId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
			if (instructorId != null)
			{
				List<string> roles = _httpContextAccessor.HttpContext.User.Claims
					.Where(c=>c.Type == ClaimTypes.Role)
					.Select(c => c.Value)
					.ToList();
				List<Courses> courses = new List<Courses>();
				if (roles.Contains("Admin")) // admin return all
				{
					courses = await _hubDbContext.Courses
						.AsNoTracking()
						.ToListAsync();
				}
				else // user return only signIn
				{
                         courses = await _hubDbContext.Courses
						.Where(c => c.UsersCourse.Any(uc => uc.UserId == instructorId))
						.AsNoTracking()
						.ToListAsync();
				}
					


				string webUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				courses.ForEach(c => c.thumbnailUrl = $"{webUrl}/{c.thumbnailUrl}");
				responseDto.Success = true;
				responseDto.Courses = courses;
				responseDto.SuccessMessage = $"Courses Loaded successfully for instructor num {instructorId}";
				return responseDto;
			}
			responseDto.Success = false;
			responseDto.Errors?.Add("Error accured while Loading the courses");
			return responseDto;
		}

		public async Task<CourseResponseDto> GetCourseByIdAsync(int courseId)
		{
			CourseResponseDto responseDto = new CourseResponseDto();

			try
			{
				int userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
				UserCourse userCourse = new UserCourse();
				userCourse.UserId = userId; 
				userCourse.CourseId = courseId;
				var course = await _courseRepository.GetByIdAsync(userCourse);
				if (course == null)
				{
					responseDto.Success = false;
					responseDto.Errors.Add("Course not found");
					return responseDto;
				}

				responseDto.Success = true;
				responseDto.Courses?.Add(course); 
				responseDto.SuccessMessage = "Course loaded successfully";

				return responseDto;
			}
			catch (Exception ex)
			{
				responseDto.Success = false;
				responseDto.Errors.Add(ex.Message);
				return responseDto;
			}
		}

		public async Task<CourseResponseDto> GetCourseByNameAsync(string courseName)
		{
			CourseResponseDto responseDto = new CourseResponseDto();

			try
			{
				int userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
				var courses = await _courseRepository.GetByNameAsync(courseName , userId);

				if (courses == null || !courses.Any())
				{
					responseDto.Success = false;
					responseDto.Errors.Add("No courses found with this name");
					return responseDto;
				}

				responseDto.Success = true;
				responseDto.Courses = courses;
				responseDto.SuccessMessage = "Courses loaded successfully";

				return responseDto;
			}
			catch (Exception ex)
			{
				responseDto.Success = false;
				responseDto.Errors.Add(ex.Message);
				return responseDto;
			}
		}

		public async Task<CourseResponseDto> RemoveCourseById(int id)
		{
			CourseResponseDto responseDto = new CourseResponseDto();

			try
			{
				UserCourse userCourse = new UserCourse();
				userCourse.UserId =int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
				userCourse.CourseId = id;
				var course = await _courseRepository.GetByIdAsync(userCourse);

				if (course == null)
				{
					responseDto.Success = false;
					responseDto.Errors.Add("Course not found");
					return responseDto;
				}

				_courseRepository.Delete(course,userCourse);

				responseDto.Success = true;
				responseDto.SuccessMessage = "Course deleted successfully";

				return responseDto;
			}
			catch (Exception ex)
			{
				responseDto.Success = false;
				responseDto.Errors.Add(ex.Message);
				return responseDto;
			}
		}

		public async Task<CourseResponseDto> UpdateCourseAsync(CourseDto courseDto)
		{
			CourseResponseDto responseDto = new CourseResponseDto();
			UserCourse userCourse = new UserCourse();
			userCourse.UserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
			userCourse.CourseId = courseDto.Id??0;
			var course = await _courseRepository.GetByIdAsync(userCourse);
			try
			{
				var _course = await _courseRepository.GetByIdAsync(userCourse);

				if (_course == null)
				{
					responseDto.Success = false;
					responseDto.Errors.Add("Course not found");
					return responseDto;
				}

				_course.Title = courseDto.Title ?? _course.Title;
				_course.Description = courseDto.Description ?? _course.Description;
				_course.CourseCode = courseDto.CourseCode ?? _course.CourseCode;

				_courseRepository.Update(_course, userCourse);

				responseDto.Success = true;
				responseDto.SuccessMessage = "Course updated successfully";

				return responseDto;
			}
			catch (Exception ex)
			{
				responseDto.Success = false;
				responseDto.Errors.Add(ex.Message);
				return responseDto;
			}
		}
		public async Task<string?> GetCourseNameByIdAsync(int? id)
		{
			if (id == null)
				return null;
			UserCourse userCourse = new UserCourse();
			userCourse.UserId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
			userCourse.CourseId = id ?? 0;
			var course = await _courseRepository.GetByIdAsync(userCourse);
			if (course != null)
				return course.Title;
			return null;
		}
	}
}
