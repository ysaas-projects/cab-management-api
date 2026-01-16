using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
        public class Cab
        {
            [Key]
            public int CabId { get; set; }
            
           [Required]
            public int? OrganizationId { get; set; }

            public string? CabType { get; set; }

            public bool? IsActive { get; set; } = true;

            public bool? IsDeleted { get; set; } = false;

            public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
        public ICollection<CabPrice> CabPrices { get; set; } = new List<CabPrice>();

    }


    // =================
    // CREATE CAB DTO
    // =================
    public class CreateCabDto
        {
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "OrganizationId must be greater than 0.")]
            public int OrganizationId { get; set; }

            [Required(ErrorMessage = "CabType is Requried")]
            [StringLength(50, ErrorMessage = "CabType must be in string and less than 50 charactor")]
            public string? CabType { get; set; }

            [Required(ErrorMessage = "IsActive Field is Requried")]
            public bool? IsActive { get; set; }

    }
        
        // =================
        // UPDATE CAB DTO
        // =================
        public class UpdateCabDto
        {
            [Key]
            public int CabId { get; set; }

            [Required(ErrorMessage = "OrganizationId  is Requried")]
            [Range(1, int.MaxValue, ErrorMessage = "OrganizationId must be greater than 0.")]
            public int? OrganizationId { get; set; }


            [Required(ErrorMessage = "CabType is Requried")]
            [StringLength(50, ErrorMessage = "CabType must be in string and less than 50 charactor")]
            public string? CabType { get; set; }

            [Required]
            public DateTime? UpdatedAt { get; set; } = DateTime.Now;

            [Required(ErrorMessage ="IsDeleted Field is Requried")]
            public bool? IsDeleted { get; set; } = false;


            [Required(ErrorMessage = "IsActive Field is Requried")]
            public bool? IsActive { get; set; }


    }
    
}
