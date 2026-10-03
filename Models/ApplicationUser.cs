using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Models
{
	public class ApplicationUser :IdentityUser<int>
	{
		public string? FullName { get; set; }
		DateTime createdAt = DateTime.Now;
		public AccountType accountType { get; set; }
		public string? NationalId { get; set; }

		public string? imgUrl { get; set; }
		public int? InstructorId { get; set; }

		//navigation properties
		virtual public ICollection<UserCourse>? UserCourses { get; set; }
		virtual public ICollection<StudentExam>? ExamResults { get; set; }
		virtual public ICollection<UserYear>? UserYears { get; set; }
	}
	[Flags]
	public enum AccountType
	{
		Teacher,
		Student,
	}
}
