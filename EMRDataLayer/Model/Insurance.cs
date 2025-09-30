using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class Insurance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(200)]
        public string InsuranceCompany { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PolicyNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GroupNumber { get; set; }

        [StringLength(50)]
        public string? PlanType { get; set; } // HMO, PPO, EPO, POS

        [StringLength(200)]
        public string? PolicyHolderName { get; set; }

        [StringLength(50)]
        public string? PolicyHolderRelationship { get; set; } // Self, Spouse, Child, Other

        [StringLength(20)]
        public string? PolicyHolderSSN { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [StringLength(100)]
        [Phone]
        public string? InsurancePhone { get; set; }

        [StringLength(500)]
        public string? InsuranceAddress { get; set; }

        public bool IsPrimary { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public ICollection<Billing> Billings { get; set; } = new List<Billing>();
    }
}