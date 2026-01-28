using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [ForeignKey("Firm")]
        public int? FirmId { get; set; }
        public virtual Firm Firm { get; set; }
        public string? FirmType { get; set; }
        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9@_\-\.]+$", ErrorMessage = "Username can only contain alphanumerics and @, _, -, .")]
        public string UserName { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        public bool EmailConfirmed { get; set; } = false;

        [StringLength(20)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        public string? MobileNumber { get; set; }

        public bool MobileNumberConfirmed { get; set; } = false;

        public byte AccessFailedCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }  // Add this line

        public Guid SecurityStamp { get; set; } = Guid.NewGuid();

        public DateTime? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; } = true;

        public bool TwoFactorEnabled { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // block for given minutes(30) min if tried wrong attemts more than attemnts(5)
        public const int MaxFailedAccessAttempts = 5;
        public static TimeSpan DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();


    }

    // Request DTOs
    public class UserCreateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public int? FirmId { get; set; }
        public bool IsActive { get; set; } = true;
        public short? RoleId { get; set; }
    }

    public class AdminUserCreateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class UserUpdateRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public int? FirmId { get; set; }
        public bool IsActive { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; }
    }

    public class AssignRoleRequest
    {
        public short RoleId { get; set; }
    }

    public class ToggleUserStatusRequest
    {
        public bool IsActive { get; set; }
    }

    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public bool IsActive { get; set; }
        public int? FirmId { get; set; }

        public List<string> Roles { get; set; } = new();
    }

}
