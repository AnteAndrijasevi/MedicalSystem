using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
    public class Examination
    {
        public Examination()
        {
            MedicalImages = new HashSet<MedicalImage>();
        }

        [Key]
        public int ExaminationId { get; set; }

        public int PatientId { get; set; }

        public int TypeId { get; set; }

        public int? DoctorId { get; set; }

        [Required]
        public DateTime ExaminationDate { get; set; }

        [Required]
        public TimeSpan ExaminationTime { get; set; }

        public string Notes { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("TypeId")]
        public virtual ExaminationType ExaminationType { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        public virtual ICollection<MedicalImage> MedicalImages { get; set; }
    }
}