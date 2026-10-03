
namespace EduTechSpace.Errors
{
	public class ApiResponse
	{
		public int statusCode {  get; set; }
		public string? message { get; set; }

		public ApiResponse(int statusCode , string? message = null)
		{
			this.statusCode = statusCode;
			this.message = message ?? GetDefoultErrorMessage(statusCode);
		}

		private string? GetDefoultErrorMessage(int statusCode)
		{
			return (statusCode) switch
			{
				401 => "Unauthorized user.You are not allowed to do this action or your token is expired",
				404 => "Resources weren't found",
				400 => "BadRequest please make sure you're sending a correct request with the correct parameters",
				500 => "Server is angry,Server may crash,if server crashed ,Dev will do shift career",
				_ => "Error"
			};
		}
	}
}
