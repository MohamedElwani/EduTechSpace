namespace EduTechSpace.Models //30/5/2026
{
	public class Errors
	{ 
		public int Id { get; set; }
		public string Message { get; set; } = null;

		public string TraceStack { get; set; } = null;
	}
}
