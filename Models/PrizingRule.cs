using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    public class PrizingRule
    {
        [Key]
        public int PrizingRuleId { get; set; }

        //[Required]
        public int FirmId { get; set; }

        //[Required]
        //[StringLength(500)]
        public string RoleDetails { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
