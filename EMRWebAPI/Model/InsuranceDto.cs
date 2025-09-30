using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class InsuranceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Insurance company is required")]
        [StringLength(200, ErrorMessage = "Insurance company cannot exceed 200 characters")]
        public string InsuranceCompany { get; set; } = string.Empty;

        [Required(ErrorMessage = "Policy number is required")]
        [StringLength(100, ErrorMessage = "Policy number cannot exceed 100 characters")]
        public string PolicyNumber { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Group number cannot exceed 100 characters")]
        public string? GroupNumber { get; set; }

        [StringLength(50, ErrorMessage = "Plan type cannot exceed 50 characters")]
        public string? PlanType { get; set; }

        [StringLength(200, ErrorMessage = "Policy holder name cannot exceed 200 characters")]
        public string? PolicyHolderName { get; set; }

        [StringLength(50, ErrorMessage = "Policy holder relationship cannot exceed 50 characters")]
        public string? PolicyHolderRelationship { get; set; }

        [StringLength(20, ErrorMessage = "Policy holder SSN cannot exceed 20 characters")]
        public string? PolicyHolderSSN { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EffectiveDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        [StringLength(100, ErrorMessage = "Insurance phone cannot exceed 100 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? InsurancePhone { get; set; }

        [StringLength(500, ErrorMessage = "Insurance address cannot exceed 500 characters")]
        public string? InsuranceAddress { get; set; }

        public bool IsPrimary { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}