using EduTechSpace.Dto;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduTechSpace.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CourseController : ControllerBase
	{
		private readonly ICourseService _courseService;
		private CookieOptions cookieOptions;
		public CourseController(ICourseService courseService)
		{
			this._courseService = courseService;
			cookieOptions = new CookieOptions
			{
				Expires = DateTime.Now.AddDays(7),
			};
		}
		[Authorize(Roles = "Teacher,Admin")]
		[HttpPost]
		public async Task<IActionResult> AddNewCourse(CourseDto course)
		{
			CourseResponseDto courseResponseDto = await _courseService.AddCourseAsync(course);
			if (courseResponseDto.Success==true)
			{
			
				return Ok(courseResponseDto);
			}
			return BadRequest(courseResponseDto);
		}
		/// <summary>
		/// EndPoint return all courses only avalible for admins
		/// </summary>
		/// <returns></returns>
		[HttpGet("get-all")]
		[Authorize(Roles = "Admin,Student,Teacher")]
		public async Task<IActionResult> GetAllCourses()// in service work on two levels admin level and userLevel
		{
			CourseResponseDto courseResponseDto = await _courseService.GetAllCoursesAsync();
			if (courseResponseDto.Success == true)
			{
				return Ok(courseResponseDto);
			}
			return BadRequest(courseResponseDto);
		}
		[HttpGet("{id:int}")] 
		[Authorize(Roles = "Teacher,Admin,Student")]
		public async Task<IActionResult> GetCourseById(int id)
		{
			CourseResponseDto courseResponseDto = await _courseService.GetCourseByIdAsync(id);
			if (courseResponseDto.Success == true)
			{
				//Save Cookie 
				Response.Cookies
					.Append("CourseId", courseResponseDto.Courses.FirstOrDefault().Id.ToString(), cookieOptions);
				return Ok(courseResponseDto);
			}
			return BadRequest(courseResponseDto);
		}
		[HttpGet("{name}")]
		[Authorize(Roles = "Teacher,admin,student")]// make two endpoints here one for admin and one for Teacher and Student access thier id from secure Token Id
		public async Task<IActionResult> GetCourseByName(string name)
		{
			CourseResponseDto courseResponseDto = await _courseService.GetCourseByNameAsync(name);
			if (courseResponseDto.Success == true)
			{
				//Save Cookie 
				Response.Cookies
					.Append("CourseId", courseResponseDto.Courses.FirstOrDefault().Id.ToString(), cookieOptions);
				return Ok(courseResponseDto);
			}
			return BadRequest(courseResponseDto);
		}
		[HttpPatch]
		[Authorize(Roles = "Teacher,Admin")] // make sure that this teacher modifies only course he own 
		public async Task<IActionResult> UpdateCourse(CourseDto courseDto)
		{
			CourseResponseDto courseResponseDto = await _courseService.UpdateCourseAsync(courseDto);
			if (courseResponseDto.Success == true)
			{
				return Ok(courseResponseDto);
			}
			return BadRequest(courseResponseDto);
		}
		[HttpDelete("{id:int}")]
		[Authorize(Roles = "Teacher,Admin")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			CourseResponseDto courseResponseDto = await _courseService.RemoveCourseById(id);
			if (courseResponseDto.Success == true)
			{
				return Ok(courseResponseDto);
			}
			return BadRequest(courseResponseDto);
		}

	}
}
