using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Models
{
	[Table("StudentExam")]
	public class StudentExam
	{
		public int StudentDegree { get; set; }
		//Navigation properties 
		virtual public ApplicationUser? Student {  get; set; }
		virtual public Exam? Exam { get; set; }
		//Forigen Keys 
		[ForeignKey("Student")]
		public int StudentId { get; set; }
		[ForeignKey("Exam")]
		public int ExamId { get; set; }
		
	}
}
