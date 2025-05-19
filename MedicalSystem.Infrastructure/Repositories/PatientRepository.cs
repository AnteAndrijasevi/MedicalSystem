using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Models;
using MedicalSystem.Infrastructure.Data;

namespace MedicalSystem.Infrastructure.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(MedicalSystemContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Patient>> SearchByNameOrOibAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Patient>();

            return await _dbSet
                .Where(p => p.LastName.Contains(searchTerm) ||
                           p.FirstName.Contains(searchTerm) ||
                           p.OIB.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<byte[]> ExportToCsvAsync(int patientId)
        {
            var patient = await _dbSet
                .Include(p => p.MedicalHistories)
                .Include(p => p.Prescriptions)
                .Include(p => p.Examinations)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null)
                return null;

            var sb = new StringBuilder();

            // Add patient info
            sb.AppendLine("Patient Information");
            sb.AppendLine($"ID,First Name,Last Name,OIB,Date of Birth,Gender,Patient Number");
            sb.AppendLine($"{patient.PatientId},{patient.FirstName},{patient.LastName},{patient.OIB},{patient.DateOfBirth:yyyy-MM-dd},{patient.Gender},{patient.PatientNumber}");

            // Add medical history
            sb.AppendLine();
            sb.AppendLine("Medical History");
            sb.AppendLine("ID,Disease Name,Start Date,End Date");
            foreach (var history in patient.MedicalHistories)
            {
                sb.AppendLine($"{history.MedicalHistoryId},{history.DiseaseName},{history.StartDate:yyyy-MM-dd},{(history.EndDate.HasValue ? history.EndDate.Value.ToString("yyyy-MM-dd") : "Present")}");
            }

            // Add examinations
            sb.AppendLine();
            sb.AppendLine("Examinations");
            sb.AppendLine("ID,Type,Date,Time,Notes");
            foreach (var exam in patient.Examinations)
            {
                sb.AppendLine($"{exam.ExaminationId},{exam.ExaminationType?.Name},{exam.ExaminationDate:yyyy-MM-dd},{exam.ExaminationTime},{exam.Notes}");
            }

            // Add prescriptions
            sb.AppendLine();
            sb.AppendLine("Prescriptions");
            sb.AppendLine("ID,Medication,Dosage,Issue Date,Instructions");
            foreach (var prescription in patient.Prescriptions)
            {
                sb.AppendLine($"{prescription.PrescriptionId},{prescription.MedicationName},{prescription.Dosage},{prescription.IssueDate:yyyy-MM-dd},{prescription.Instructions}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}