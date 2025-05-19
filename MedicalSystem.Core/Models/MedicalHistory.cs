using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
    public class MedicalHistory
    {
        [Key]
        public int MedicalHistoryId { get; set; }

        public int PatientId { get; set; }

        [Required]
        [StringLength(200)]
        public string DiseaseName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // Navigation property
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
    }
}