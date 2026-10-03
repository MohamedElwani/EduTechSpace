using System.Globalization;

namespace EduTechSpace.Errors
{
	public class ApiExceptionResponse : ApiResponse
	{
		public string ? details { get; set; }
		public ApiExceptionResponse(int statusCode, string? message = null ,string ? details =null) 
			: base(statusCode,message)
		{
			this.details = details;
		}
	}
}
