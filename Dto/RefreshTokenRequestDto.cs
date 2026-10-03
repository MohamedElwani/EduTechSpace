using System.ComponentModel.DataAnnotations;

namespace EduTechSpace.Dto
{
    /// <summary>
    /// Input DTO for the POST api/account/refresh-token endpoint.
    /// </summary>
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "RefreshToken is required.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
