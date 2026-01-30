using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cab_management.Models
{
    // =====================================================
    // DUTY EXPENSES ENTITY (DB TABLE)
    // =====================================================
    public class DutyExpense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DutyExpenseId { get; set; }
		public int? FirmId { get; set; }

		[Required]
        public int DutyId { get; set; }

        [Required(ErrorMessage = "Expense type is required")]
        [StringLength(100, MinimumLength = 2)]
        public string ExpenseType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // VARCHAR EXPENSE AMOUNT
        [Required(ErrorMessage = "Expense amount is required")]

        public string ExpenseAmount { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    // =====================================================
    // CREATE DUTY EXPENSE DTO
    // =====================================================
    public class CreateDutyExpenseDTO
    {
        [Required]
        public int DutyId { get; set; }

        [Required(ErrorMessage = "Expense type is required")]
        [StringLength(100, MinimumLength = 2)]
        public string ExpenseType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }


        [Required(ErrorMessage = "Expense amount is required")]
        public string ExpenseAmount { get; set; } = string.Empty;
    }

    // =====================================================
    // UPDATE DUTY EXPENSE DTO
    // =====================================================
    public class UpdateDutyExpenseDTO
    {
        public int DutyId { get; set; }

        [Required(ErrorMessage = "Expense type is required")]
        [StringLength(100, MinimumLength = 2)]
        public string ExpenseType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }


        [Required(ErrorMessage = "Expense amount is required")]

        public string ExpenseAmount { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
    }
}