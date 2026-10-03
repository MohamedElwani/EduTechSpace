using EduTechSpace.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Models
{
	[Table("UserYears")]
	public class UserYear
	{
		public int id { get; set; }
		public Levels UserLevel { get; set; }

		//Navigation properties 
		public virtual ApplicationUser User { get; set; }
		//Forigen Key
		[ForeignKey("User")]
		public int UserId { get; set; }
	}
}
