using cab_management.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriverDetails.Models
{

    public class DriverDetail
    {
        public int DriverDetailId { get; set; }

        [ForeignKey(nameof(Firm))]
        public int FirmId { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Firm Firm { get; set; }
        public User User { get; set; }
    }

    // ======================
    // CREATE DTO
    // ======================
    public class AddDriverDetailDTO
    {
        [Required]
        public string DriverName { get; set; }

        [Required]
        [StringLength(13)]
        public string MobileNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // ======================
    // UPDATE DTO
    // ======================
    public class UpdateDriverDetailDTO
    {
        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
        public bool? IsActive { get; set; }
    }

    // ======================
    // RESPONSE DTO
    // ======================
    public class DriverDetailResponseDTO
    {
        public int DriverDetailId { get; set; }
        public string DriverName { get; set; }
        public string MobileNumber { get; set; }
        public bool IsActive { get; set; }
        public string FirmName { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}