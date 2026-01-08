using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
    // =====================================================
    // ORGANIZATIONTERMS ENTITY (DB TABLE)
    // =====================================================
    public class OrganizationTerm
    {
        [Key]
        public int OrganizationtermId { get; set; }

        [Required(ErrorMessage = "OrganizationId is required")]
        public int OrganizationId { get; set; }

        [Required(ErrorMessage = "Minimum 5 character and maximum 500 character")]
        [StringLength(500, MinimumLength = 5)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsDeleted { get; set; } = false;
    }
    // =====================================================
    // CREATE ORGANIZATIONTERMS DTO
    // =====================================================
    public class CreateTermDto
    {
        [Key]
        public int OrganizationtermId { get; set; }

        [Required(ErrorMessage = "OrganizationId is required")]
        public int OrganizationId { get; set; }

        [Required(ErrorMessage = "Minimum 5 character and maximum 500 character")]
        [StringLength(500, MinimumLength = 5)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }

    }
    // =====================================================
    // UPDATE ORGANIZATIONTERMS DTO
    // =====================================================
    public class UpdateTermDto
    {
        [Key]
        public int OrganizationtermId { get; set; }

        [Required(ErrorMessage = "OrganizationId is required")]
        public int OrganizationId { get; set; }

        [Required(ErrorMessage = "Minimum 5 character and maximum 500 character")]
        [StringLength(500, MinimumLength = 5)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    }
}
