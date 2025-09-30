using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class PrescriptionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Medication ID is required")]
        public int MedicationId { get; set; }

        [StringLength(200, ErrorMessage = "Medication name cannot exceed 200 characters")]
        public string? MedicationName { get; set; }

        [Required(ErrorMessage = "Provider ID is required")]
        public int ProviderId { get; set; }

        [StringLength(100, ErrorMessage = "Provider name cannot exceed 100 characters")]
        public string? ProviderName { get; set; }

        [Required(ErrorMessage = "Dosage is required")]
        [StringLength(200, ErrorMessage = "Dosage cannot exceed 200 characters")]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Frequency is required")]
        [StringLength(200, ErrorMessage = "Frequency cannot exceed 200 characters")]
        public string Frequency { get; set; } = string.Empty;

        [Required(ErrorMessage = "Route is required")]
        [StringLength(200, ErrorMessage = "Route cannot exceed 200 characters")]
        public string Route { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10000")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Refills is required")]
        [Range(0, 12, ErrorMessage = "Refills must be between 0 and 12")]
        public int Refills { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [StringLength(500, ErrorMessage = "Instructions cannot exceed 500 characters")]
        public string? Instructions { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Active";

        public bool IsGenericAllowed { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}