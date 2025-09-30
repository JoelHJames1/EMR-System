using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRDataLayer.Model
{
    public class BillingItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BillingId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? CPTCode { get; set; } // Current Procedural Terminology

        [StringLength(50)]
        public string? ICDCode { get; set; } // International Classification of Diseases

        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Billing Billing { get; set; } = null!;
    }
}