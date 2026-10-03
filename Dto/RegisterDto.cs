using EduTechSpace.Models;
using EduTechSpace.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace EduTechSpace.Dto
{
	public class RegisterDto
	{
		public string fullName { get; set; }
		public string userName {  get; set; }
		[RegularExpression(@"^\d{14}$",ErrorMessage ="NationalID must be consist of 14 digits exatly")]
		public string NationalId { get; set; }
		[EmailAddress]
		public string email { get; set; }
		public string password { get; set; }
		[Compare("password")]
		public string ConfirmPassword { get; set; }
		public AccountType AccountType { get; set; }

		public Levels UserLevel { get; set; }
		public IFormFile? files { get; set; }
	}
}
