using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRDataLayer.Model
{
    public class VitalSign
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime MeasurementDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Temperature { get; set; } // in Fahrenheit

        [StringLength(20)]
        public string? TemperatureUnit { get; set; } = "F";

        public int? SystolicBP { get; set; }

        public int? DiastolicBP { get; set; }

        public int? HeartRate { get; set; } // beats per minute

        public int? RespiratoryRate { get; set; } // breaths per minute

        [Column(TypeName = "decimal(5,2)")]
        public decimal? OxygenSaturation { get; set; } // percentage

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Height { get; set; } // in inches or cm

        [StringLength(20)]
        public string? HeightUnit { get; set; } = "in";

        [Column(TypeName = "decimal(6,2)")]
        public decimal? Weight { get; set; } // in lbs or kg

        [StringLength(20)]
        public string? WeightUnit { get; set; } = "lbs";

        [Column(TypeName = "decimal(5,2)")]
        public decimal? BMI { get; set; }

        [StringLength(200)]
        public string? MeasuredBy { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Patient Patient { get; set; } = null!;
    }
}