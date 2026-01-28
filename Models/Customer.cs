using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
    [Table("Customer")] 
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public int FirmId { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        public string LogoImagePath { get; set; }

        public string Address { get; set; }

        [StringLength(20)]
        public string GstNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey("FirmId")]
        public Firm Firm { get; set; }
    }

    // ==============================
    // CRETAE CUSTOMER DTO 
    // ==============================
    public class CustomerCreateDto
    {
        

        [Required]
        public string CustomerName { get; set; }

        public string Address { get; set; }

        public string GstNumber { get; set; }

        public IFormFile LogoImage { get; set; }
    }

    // ==============================
    // UPDATE CUSTOMER DTO 
    // ==============================
    public class CustomerUpdateDto
    {

        [Required]
        public string CustomerName { get; set; }

        public string Address { get; set; }

        public string GstNumber { get; set; }

        public bool IsActive { get; set; } = true;
        public IFormFile? LogoImage { get; set; } 


    }
    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }
        public int FirmId { get; set; }
        public string? FirmName { get; set; }
        public string CustomerName { get; set; }
        public string? Address { get; set; }
        public string? GstNumber { get; set; }
        public string? LogoImagePath { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
