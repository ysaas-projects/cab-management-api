using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    // ================================
    // ENTITY: DutySlipCustomerUser
    // ================================
    public class DutySlipCustomerUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DutySlipCustomerUserId { get; set; }

        // ---------------- FOREIGN KEYS (LOGICAL)
        [Required]
        [ForeignKey(nameof(DutySlip))]
        public int DutySlipId { get; set; }

        [Required]
        [ForeignKey(nameof(CustomerUser))]
        public int CustomerUserId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DutySlip? DutySlip { get; set; }

        public CustomerUser? CustomerUser { get; set; }
    }

    public class DutySlipCustomerUserCreateDto
    {
        [Required]
        public int DutySlipId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Select at least one customer user")]
        public List<int> CustomerUserIds { get; set; } = new();
    }
}
