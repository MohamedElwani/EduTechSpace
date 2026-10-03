using System.ComponentModel.DataAnnotations;

namespace EduTechSpace.Dto
{
    /// <summary>
    /// Input DTO for assigning a role to a user identified by their National ID (SSN).
    /// </summary>
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "NationalId (SSN) is required.")]
        [StringLength(20, MinimumLength = 5,
            ErrorMessage = "NationalId must be between 5 and 20 characters.")]
        [RegularExpression(@"^\d+$",
            ErrorMessage = "NationalId must contain digits only.")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "RoleName is required.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "RoleName must be between 2 and 50 characters.")]
        public string RoleName { get; set; } = string.Empty;
    }
}
