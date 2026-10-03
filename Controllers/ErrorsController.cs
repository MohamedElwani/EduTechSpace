using EduTechSpace.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduTechSpace.Controllers
{
	[Route("errors/{code}")]
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ErrorsController : ControllerBase
	{

		public ActionResult Error(int code)
		{
			return (code) switch
			{
				401 => Unauthorized(new ApiResponse(401, "Access token is missing, invalid, or expired. Please use your refresh token to get a new one.")),
				403 => StatusCode(403, new ApiResponse(403, "You do not have the required role or permissions to perform this action.")),
				404 => NotFound(new ApiResponse(404)),
				_ => StatusCode(code)
			};
		}
	}
}
