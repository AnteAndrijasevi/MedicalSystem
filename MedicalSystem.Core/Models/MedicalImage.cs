using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalSystem.Core.Models
{
    public class MedicalImage
    {
        [Key]
        public int ImageId { get; set; }

        public int ExaminationId { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [StringLength(50)]
        public string FileType { get; set; }

        // Navigation property
        [ForeignKey("ExaminationId")]
        public virtual Examination Examination { get; set; }
    }
}