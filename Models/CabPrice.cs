using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
    public class CabPrice
    {
        [Key]
        public int CabPriceId { get; set; }

        public int? FirmId { get; set; }

        public int? PriceRuleId { get; set; }

        public double? Price { get; set; }

        public bool? IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool? IsDeleted { get; set; } = false;
    }

    public class CreateCabPriceDTO
    {
        [Required(ErrorMessage = "FirmId Field is Requried")]
        [Range(1, int.MaxValue, ErrorMessage = "FirmId  must greater than 0")]
        public int FirmId { get; set; }

        [Required(ErrorMessage = "PriceRuleId Field Is Requried")]
        [Range(1, int.MaxValue, ErrorMessage = "PriceRuleId must greater than 0")]
        public int PriceRuleId { get; set; }

        [Required(ErrorMessage = "price Field is Requried")]
        [Range(1, double.MaxValue, ErrorMessage = "Proper Price is Requried")]
        public double Price { get; set; }

        [Required(ErrorMessage = "IsActive Field is Requried")]
        public bool? IsActive { get; set; } = true;

        [Required(ErrorMessage = "CreatedAt Field is Requried")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

    }

    public class UpdateCabPriceDTO
    {
        [Key]
        public int CabPriceId { get; set; }

        [Required(ErrorMessage = "FirmId Field is Requried")]
        [Range(1, int.MaxValue, ErrorMessage = "FirmId must greater than 0")]
        public int? FirmId { get; set; }


        [Required(ErrorMessage = "PriceRuleId Field is Requried")]
        [Range(1, int.MaxValue, ErrorMessage = "PriceRuleId must greater than 0")]
        public int? PriceRuleId { get; set; }


        [Required(ErrorMessage = "Price Field is Requried")]
        [Range(1, double.MaxValue, ErrorMessage = "Proper Price is Requried")]
        public double? Price { get; set; }


        [Required(ErrorMessage = "IsActive Field is Requried")]
        public bool? IsActive { get; set; } = true;


        [Required(ErrorMessage = "UpdatedAt Field is Requried")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "IsDeleted Field is Requried")]
        public bool? IsDeleted { get; set; } = false;

    }


}
