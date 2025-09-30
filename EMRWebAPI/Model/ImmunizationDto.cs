using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class ImmunizationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Vaccine name is required")]
        [StringLength(200, ErrorMessage = "Vaccine name cannot exceed 200 characters")]
        public string VaccineName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "CVX code cannot exceed 100 characters")]
        public string? CVXCode { get; set; }

        [StringLength(50, ErrorMessage = "Dose number cannot exceed 50 characters")]
        public string? DoseNumber { get; set; }

        [Required(ErrorMessage = "Administered date is required")]
        [DataType(DataType.Date)]
        public DateTime AdministeredDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        [StringLength(100, ErrorMessage = "Lot number cannot exceed 100 characters")]
        public string? LotNumber { get; set; }

        [StringLength(200, ErrorMessage = "Manufacturer cannot exceed 200 characters")]
        public string? Manufacturer { get; set; }

        [StringLength(100, ErrorMessage = "Administered by cannot exceed 100 characters")]
        public string? AdministeredBy { get; set; }

        [StringLength(100, ErrorMessage = "Site cannot exceed 100 characters")]
        public string? Site { get; set; }

        [StringLength(100, ErrorMessage = "Route cannot exceed 100 characters")]
        public string? Route { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}