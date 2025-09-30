using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRDataLayer.Model
{
    public class Billing
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Unpaid"; // Unpaid, Partial, Paid, Overdue

        [StringLength(100)]
        public string? PaymentMethod { get; set; }

        public DateTime? PaymentDate { get; set; }

        public int? InsuranceId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? InsuranceCoverage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PatientResponsibility { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Insurance? Insurance { get; set; }
        public ICollection<BillingItem> BillingItems { get; set; } = new List<BillingItem>();
    }
}