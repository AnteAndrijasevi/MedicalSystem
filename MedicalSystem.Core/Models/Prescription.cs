using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        public int PatientId { get; set; }

        [Required]
        [StringLength(200)]
        public string MedicationName { get; set; }

        [Required]
        [StringLength(100)]
        public string Dosage { get; set; }

        public string Instructions { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

    }
}