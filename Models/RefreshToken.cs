using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTechSpace.Models
{
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The opaque refresh token string sent to / received from the client.
        /// </summary>
        [Required]
        public string Token { get; set; } = string.Empty;

        /// <summary>When this refresh token stops being valid.</summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>Set to true once this token has been exchanged for a new pair.</summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>Set to true when the token is manually revoked (logout / security event).</summary>
        public bool IsRevoked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── FK to ApplicationUser ───────────────────────────────────────────
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        // ── Computed helpers ────────────────────────────────────────────────
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        [NotMapped]
        public bool IsActive => !IsRevoked && !IsUsed && !IsExpired;
    }
}
