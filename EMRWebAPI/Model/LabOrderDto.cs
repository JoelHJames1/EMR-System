using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class LabOrderDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [StringLength(100, ErrorMessage = "Patient name cannot exceed 100 characters")]
        public string? PatientName { get; set; }

        [Required(ErrorMessage = "Provider ID is required")]
        public int ProviderId { get; set; }

        [StringLength(100, ErrorMessage = "Provider name cannot exceed 100 characters")]
        public string? ProviderName { get; set; }

        [Required(ErrorMessage = "Test name is required")]
        [StringLength(200, ErrorMessage = "Test name cannot exceed 200 characters")]
        public string TestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Test code is required")]
        [StringLength(100, ErrorMessage = "Test code cannot exceed 100 characters")]
        public string TestCode { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "LOINC code cannot exceed 100 characters")]
        public string? LOINCCode { get; set; }

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [StringLength(50, ErrorMessage = "Priority cannot exceed 50 characters")]
        public string Priority { get; set; } = "Routine";

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Ordered";

        [Required(ErrorMessage = "Ordered date is required")]
        [DataType(DataType.DateTime)]
        public DateTime OrderedDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CollectedDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CompletedDate { get; set; }

        [StringLength(100, ErrorMessage = "Specimen cannot exceed 100 characters")]
        public string? Specimen { get; set; }

        [StringLength(500, ErrorMessage = "Special instructions cannot exceed 500 characters")]
        public string? SpecialInstructions { get; set; }

        [StringLength(500, ErrorMessage = "Clinical notes cannot exceed 500 characters")]
        public string? ClinicalNotes { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}