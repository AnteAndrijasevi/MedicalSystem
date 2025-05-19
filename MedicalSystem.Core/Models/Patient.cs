using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Core.Models
{
    public class Patient
    {
        public Patient()
        {
            MedicalHistories = new HashSet<MedicalHistory>();
            Examinations = new HashSet<Examination>();
            Prescriptions = new HashSet<Prescription>();
        }

        [Key]
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(11)]
        public string OIB { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(1)]
        public string Gender { get; set; }

        [StringLength(20)]
        public string PatientNumber { get; set; }

        [StringLength(20)]
        public string ContactNumber { get; set; }

        // Navigation properties
        public virtual ICollection<MedicalHistory> MedicalHistories { get; set; }
        public virtual ICollection<Examination> Examinations { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
    }
}