using System.Text.Json.Serialization;

namespace EduTechSpace.Dto
{
	public class AuthResponseDto
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? UserId { get; set; } = null;
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string Message { get; set; } = string.Empty;
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? accessToken {  get; set; }
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public DateTime? accessTokenExpireDate { get; set; }
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? refreshToken { get; set; }
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public DateTime? refreshTokenExpiresAt { get; set; }

		public List<string> errors { get; set; } = new List<string>();
	}
}
