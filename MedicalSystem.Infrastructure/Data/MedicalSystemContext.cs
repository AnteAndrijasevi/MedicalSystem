using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MedicalSystem.Infrastructure.Data
{
    public class MedicalSystemContext : DbContext
    {
        public MedicalSystemContext(DbContextOptions<MedicalSystemContext> options)
            : base(options)
        {
            // Enable lazy loading
            ChangeTracker.LazyLoadingEnabled = true;
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }
        public DbSet<ExaminationType> ExaminationTypes { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<MedicalImage> MedicalImages { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExaminationType>()
                .Property(e => e.Description)
                .IsRequired(false);

            // Configure DateTime properties to use UTC
            modelBuilder.Entity<Patient>()
                .Property(p => p.DateOfBirth)
                .HasConversion(
                    v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            // Configure indexes and relationships
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.OIB)
                .IsUnique();

            // Seed examination types
            modelBuilder.Entity<ExaminationType>().HasData(
                new ExaminationType { TypeId = 1, Code = "GP", Name = "Opći tjelesni pregled", Description = "Kompletan pregled cijelog tijela" },
                new ExaminationType { TypeId = 2, Code = "KRV", Name = "Test krvi", Description = "Analiza krvne slike" },
                new ExaminationType { TypeId = 3, Code = "X-RAY", Name = "Rendgensko skeniranje", Description = "Snimanje rendgenskim zrakama" },
                new ExaminationType { TypeId = 4, Code = "CT", Name = "CT sken", Description = "Kompjuterizirana tomografija" },
                new ExaminationType { TypeId = 5, Code = "MR", Name = "MRI sken", Description = "Magnetska rezonanca" },
                new ExaminationType { TypeId = 6, Code = "ULTRA", Name = "Ultrazvuk", Description = "Pregled ultrazvukom" },
                new ExaminationType { TypeId = 7, Code = "EKG", Name = "Elektrokardiogram", Description = "Mjerenje električne aktivnosti srca" },
                new ExaminationType { TypeId = 8, Code = "ECHO", Name = "Ehokardiogram", Description = "Ultrazvuk srca" },
                new ExaminationType { TypeId = 9, Code = "EYE", Name = "Pregled očiju", Description = "Oftalmološki pregled" },
                new ExaminationType { TypeId = 10, Code = "DERM", Name = "Dermatološki pregled", Description = "Pregled kože" },
                new ExaminationType { TypeId = 11, Code = "DENTA", Name = "Pregled zuba", Description = "Stomatološki pregled" },
                new ExaminationType { TypeId = 12, Code = "MAMMO", Name = "Mamografija", Description = "Radiološki pregled dojki" },
                new ExaminationType { TypeId = 13, Code = "NEURO", Name = "Neurološki pregled", Description = "Pregled neurološkog sustava" }
            );
        }
    }
}