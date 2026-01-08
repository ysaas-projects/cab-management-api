using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DriverDetails.Models;

// =====================================================
// DRIVERDETAILS (DB TABLE)
// =====================================================
public partial class DriverDetail
{
    public int DriverDetailId { get; set; }

    public int? FirmId { get; set; }

    public int? UserId { get; set; }

    public string? DriverName { get; set; }

    public string? MobileNumber { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }
}

// =====================================================
// CREATE DRIVER DETAILS DTO
// =====================================================
public class AddDriverDetailDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "FirmID must be greater than 0.")] 
    public int? FirmId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0.")]
    public int? UserId { get; set; }

    [Required(ErrorMessage = "DriverName is Requried")]
    public string? DriverName { get; set; }

    [Required(ErrorMessage = "Mobile Number is Requried")]
    [StringLength(13, ErrorMessage = "Mobile Number must be in 13")]
    public string? MobileNumber { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public bool? IsDeleted { get; set; }
}

// =====================================================
// UPDATE DRIVER DETAILS DTO
// =====================================================
public class UpdateDriverDetailDTO
{
    public int DriverDetailId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "FirmID must be greater than 0.")]
    public int? FirmId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0.")]
    public int? UserId { get; set; }

    [Required(ErrorMessage = "DriverName is Requried")]
    public string? DriverName { get; set; }

    [Required(ErrorMessage = "Mobile Number is Requried")]
    [StringLength(13, ErrorMessage = "Mobile Number must be in 13")]
    public string? MoblieNumber { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    public bool? IsDeleted { get; set; }
}

