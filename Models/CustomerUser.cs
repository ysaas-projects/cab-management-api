using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    [Table("CustomerUsers")]

    public class CustomerUser
    {
        [Key]
        public int CustomerUserId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(20)]
        public string MobileNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }

    public class CreateCustomerUserDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public string UserName { get; set; }

        public string MobileNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateCustomerUserDto
    {
        [Required]
        public string UserName { get; set; }

        public string MobileNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
