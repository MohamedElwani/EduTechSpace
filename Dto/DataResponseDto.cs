using EduTechSpace.Models;

namespace EduTechSpace.Dto
{
	public class DataResponseDto<T>
	{
			public List<T> Data { get; set; } = new List<T>();
			public bool? Success { get; set; }
			public string? SuccessMessage { get; set; }
			public List<string> Errors { get; set; } = new List<string>();
		
	}
}
