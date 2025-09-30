using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class LabResultDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lab order ID is required")]
        public int LabOrderId { get; set; }

        [Required(ErrorMessage = "Test component is required")]
        [StringLength(200, ErrorMessage = "Test component cannot exceed 200 characters")]
        public string TestComponent { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Value cannot exceed 100 characters")]
        public string? Value { get; set; }

        [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
        public string? Unit { get; set; }

        [StringLength(100, ErrorMessage = "Reference range cannot exceed 100 characters")]
        public string? ReferenceRange { get; set; }

        [StringLength(50, ErrorMessage = "Flag cannot exceed 50 characters")]
        public string? Flag { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ResultDate { get; set; }

        public string? Comments { get; set; }

        [StringLength(100, ErrorMessage = "Performed by cannot exceed 100 characters")]
        public string? PerformedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}