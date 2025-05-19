using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicalSystem.Core.Models;

namespace MedicalSystem.Infrastructure.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<IEnumerable<Patient>> SearchByNameOrOibAsync(string searchTerm);
        Task<byte[]> ExportToCsvAsync(int patientId);
    }
}