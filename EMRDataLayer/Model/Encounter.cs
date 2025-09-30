using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// HL7 FHIR Encounter - Interaction between patient and healthcare provider
    /// </summary>
    public class Encounter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EncounterNumber { get; set; } = string.Empty;

        [Required]
        public int PatientId { get; set; }

        public int? ProviderId { get; set; }

        [Required]
        [StringLength(50)]
        public string EncounterClass { get; set; } = string.Empty; // Inpatient, Outpatient, Emergency, Ambulatory

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Planned"; // Planned, Arrived, InProgress, Finished, Cancelled

        [StringLength(100)]
        public string? EncounterType { get; set; } // Consultation, Follow-up, Emergency, etc.

        [StringLength(50)]
        public string? Priority { get; set; } // Routine, Urgent, Emergency, ASAP

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(500)]
        public string? ReasonForVisit { get; set; }

        public int? LocationId { get; set; }

        public int? DepartmentId { get; set; }

        [StringLength(50)]
        public string? AdmissionSource { get; set; } // Emergency, Transfer, Referral

        [StringLength(50)]
        public string? DischargeDisposition { get; set; } // Home, Transfer, Expired

        public DateTime? AdmissionDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        public int? ReferralId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Provider? Provider { get; set; }
        public Location? Location { get; set; }
        public Department? Department { get; set; }
        public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        public ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
        public ICollection<Observation> Observations { get; set; } = new List<Observation>();
    }
}