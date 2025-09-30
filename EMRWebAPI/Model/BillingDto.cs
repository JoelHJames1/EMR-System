using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class BillingDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [StringLength(100, ErrorMessage = "Patient name cannot exceed 100 characters")]
        public string? PatientName { get; set; }

        public int? EncounterId { get; set; }

        [Required(ErrorMessage = "Invoice number is required")]
        [StringLength(50, ErrorMessage = "Invoice number cannot exceed 50 characters")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Invoice date is required")]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Total amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0")]
        public decimal TotalAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Paid amount must be greater than or equal to 0")]
        public decimal PaidAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Balance amount must be greater than or equal to 0")]
        public decimal BalanceAmount { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Unpaid";

        [StringLength(100, ErrorMessage = "Payment method cannot exceed 100 characters")]
        public string? PaymentMethod { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }

        public int? InsuranceId { get; set; }

        public decimal? InsuranceCoverage { get; set; }

        public decimal? PatientResponsibility { get; set; }

        public string? Notes { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}