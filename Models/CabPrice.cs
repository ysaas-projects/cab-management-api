using System.ComponentModel.DataAnnotations;

namespace cab_management.Models
{
    // =======================
    // MAIN ENTITY [DB TABLE]
    // =======================
    public partial class CabPrice
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

    // ====================
    // CREATE CABPRICE
    // ====================
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

        [Required(ErrorMessage = "CreatedAt Field is Requried")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "IsActive Field is Requried")]
        public bool? IsActive { get; set; } = true;



    }


    // =====================
    // UPDATE CABPRICE
    // =====================
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


        [Required(ErrorMessage = "UpdatedAt Field is Requried")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "IsActive Field is Requried")]
        public bool? IsActive { get; set; } = true;

        [Required(ErrorMessage = "IsDeleted Field is Requried")]
        public bool? IsDeleted { get; set; } = false;







    }

}
