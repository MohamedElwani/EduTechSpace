using EduTechSpace.Context;
using EduTechSpace.Errors;
using System.Net;
using System.Text.Json;

namespace EduTechSpace.Middlewares
{
	//30/5/2026
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private EduHubDbContext _context = new EduHubDbContext();

		public ExceptionMiddleware(RequestDelegate next
			,IWebHostEnvironment webHostEnvironment
			,ILogger<ExceptionMiddleware> logger)
			
		{
			_next = next;
			_webHostEnvironment = webHostEnvironment;
			_logger = logger;
		}
		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				// handel request here 

				await _next.Invoke(httpContext);
				// handel response here 

			}
			catch (Exception ex)
			{
				
				if(_webHostEnvironment.IsDevelopment())
				{
					_logger.LogError(ex.Message);
				}
				else
				{
					var exceptionError = new EduTechSpace.Models.Errors();
					exceptionError.Message = ex.Message;
					exceptionError.TraceStack = ex.StackTrace.ToString();
					_context.Errors.Add(exceptionError);
					_context.SaveChanges();
				}
				httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
				httpContext.Response.ContentType = "application/json";

				var response = (_webHostEnvironment.IsDevelopment())?
					new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
					: new ApiResponse((int)HttpStatusCode.InternalServerError);

				var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var responseInJson = JsonSerializer.Serialize(response, jsonOptions);

				await httpContext.Response.WriteAsync(responseInJson);
			}

		}
	}
}
