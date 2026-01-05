using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
    
        public  class Cab
        {
            [Key]
            public int CabId { get; set; }
            
           [Required]
            public int? OrganizationId { get; set; }


            public string? CabType { get; set; }



            public bool? IsActive { get; set; } = true;



            public bool? IsDeleted { get; set; } = false;

            public DateTime? CreatedAt { get; set; } = DateTime.Now;

            public DateTime? UpdetedAt { get; set; }
        }
        public class CreateCabDto
        {
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "OrganizationId must be greater than 0.")]
            //[SwaggerSchema(Description ="OrganizationId is Unique ", Nullable =false)]
            public int OrganizationId { get; set; }

            [Required]

            public bool? IsActive { get; set; }

            [Required(ErrorMessage = "CabType is Requried")]
            [StringLength(50, ErrorMessage = "CabType must be in string and less than 50 charactor")]
            public string? CabType { get; set; }

        }
        public class UpdateCabDto
        {
            [Key]
            public int CabId { get; set; }


            [Required(ErrorMessage = "CabType is Requried")]
            [StringLength(50, ErrorMessage = "CabType must be in string and less than 50 charactor")]
            public string? CabType { get; set; }

            [Required]
            public bool? IsActive { get; set; }
            [Required]
            public DateTime? UpdatedAt { get; set; }


            public bool? IsDeleted { get; set; } = false;

            public int? OrganizationId { get; set; }
        }
    
}
