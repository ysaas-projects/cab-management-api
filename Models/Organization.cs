using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    // =====================================================
    // ORGANIZATION ENTITY (DB TABLE)
    // =====================================================
    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrganizationId { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string OrganizationName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? LogoImagePath { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 5)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Mobile number must be a valid 10-digit Indian mobile number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        [RegularExpression(
            @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$",
            ErrorMessage = "Invalid GST number format")]
        public string GstNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }

    // =====================================================
    // CREATE ORGANIZATION DTO
    // =====================================================
    public class CreateOrganizationDTO
    {
        [Required(ErrorMessage = "Organization name is required")]
        [StringLength(150, MinimumLength = 2)]
        public string OrganizationName { get; set; } = string.Empty;

        // File Upload
        public IFormFile? LogoImage { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250, MinimumLength = 5)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter valid 10-digit Indian mobile number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "GST number is required")]
        [StringLength(15)]
        [RegularExpression(
            @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$",
            ErrorMessage = "Invalid GST format")]
        public string GstNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    // =====================================================
    // UPDATE ORGANIZATION DTO
    // =====================================================
    public class UpdateOrganizationDTO
    {
        [Required(ErrorMessage = "Organization name is required")]
        [StringLength(150, MinimumLength = 2)]
        public string OrganizationName { get; set; } = string.Empty;

        // File Upload (Optional)
        public IFormFile? LogoImage { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250, MinimumLength = 5)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter valid 10-digit Indian mobile number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "GST number is required")]
        [StringLength(15)]
        [RegularExpression(
            @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$",
            ErrorMessage = "Invalid GST format")]
        public string GstNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
