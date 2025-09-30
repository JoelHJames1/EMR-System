using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    /// <summary>
    /// HL7 FHIR CarePlan DTO - Comprehensive treatment and care plan
    /// </summary>
    public class CarePlanDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [StringLength(100, ErrorMessage = "Patient name cannot exceed 100 characters")]
        public string? PatientName { get; set; }

        [Required(ErrorMessage = "Provider ID is required")]
        public int ProviderId { get; set; }

        public int? EncounterId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Active";

        [StringLength(50, ErrorMessage = "Intent cannot exceed 50 characters")]
        public string Intent { get; set; } = "Plan";

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public string? Goals { get; set; }

        public string? ActivitiesSummary { get; set; }

        public string? Medications { get; set; }

        public string? DietRestrictions { get; set; }

        public string? ExerciseRecommendations { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [StringLength(36, ErrorMessage = "Created by cannot exceed 36 characters")]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}