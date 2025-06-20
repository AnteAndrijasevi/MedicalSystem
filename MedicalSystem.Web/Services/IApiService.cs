using MedicalSystem.API.DTOs;

namespace MedicalSystem.Web.Services
{
    public interface IApiService
    {
        Task<IEnumerable<PatientDto>> GetPatientsAsync();
        Task<PatientDto> GetPatientAsync(int id);
        Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm);
        Task<PatientDto> CreatePatientAsync(PatientDto patient);
        Task<bool> UpdatePatientAsync(int id, PatientDto patient);
        Task<bool> DeletePatientAsync(int id);
        Task<byte[]> ExportPatientAsync(int id);

        // Dodaj ove metode za ostale entitete
        Task<IEnumerable<ExaminationDto>> GetExaminationsAsync();
        Task<IEnumerable<ExaminationDto>> GetExaminationsByPatientAsync(int patientId);
        Task<ExaminationDto> CreateExaminationAsync(ExaminationDto examination);

        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsAsync();
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByPatientAsync(int patientId);
        Task<PrescriptionDto> CreatePrescriptionAsync(PrescriptionDto prescription);

        Task<IEnumerable<MedicalHistoryDto>> GetMedicalHistoriesAsync();
        Task<IEnumerable<MedicalHistoryDto>> GetMedicalHistoriesByPatientAsync(int patientId);
        Task<MedicalHistoryDto> CreateMedicalHistoryAsync(MedicalHistoryDto history);
    }
}