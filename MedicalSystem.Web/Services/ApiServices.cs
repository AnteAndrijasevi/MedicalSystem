using MedicalSystem.API.DTOs;
using MedicalSystem.Web.Services;
using System.Text;
using System.Text.Json;

namespace MedicalSystem.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MedicalSystemAPI");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        #region Patients

        public async Task<IEnumerable<PatientDto>> GetPatientsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/patients");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var patients = JsonSerializer.Deserialize<List<PatientDto>>(jsonContent, _jsonOptions);

                return patients ?? new List<PatientDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting patients: {ex.Message}");
                return new List<PatientDto>();
            }
        }

        public async Task<PatientDto> GetPatientAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/patients/{id}");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PatientDto>(jsonContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting patient {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/patients/search?term={Uri.EscapeDataString(searchTerm)}");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var patients = JsonSerializer.Deserialize<List<PatientDto>>(jsonContent, _jsonOptions);

                return patients ?? new List<PatientDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching patients: {ex.Message}");
                return new List<PatientDto>();
            }
        }

        public async Task<PatientDto> CreatePatientAsync(PatientDto patient)
        {
            try
            {
                var json = JsonSerializer.Serialize(patient, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/patients", content);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PatientDto>(jsonContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating patient: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdatePatientAsync(int id, PatientDto patient)
        {
            try
            {
                patient.PatientId = id;

                var json = JsonSerializer.Serialize(patient, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/patients/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating patient: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/patients/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting patient: {ex.Message}");
                return false;
            }
        }

        public async Task<byte[]> ExportPatientAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/patients/{id}/export");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting patient: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Examinations

        public async Task<IEnumerable<ExaminationDto>> GetExaminationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/examinations");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ExaminationDto>>(jsonContent, _jsonOptions) ?? new List<ExaminationDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting examinations: {ex.Message}");
                return new List<ExaminationDto>();
            }
        }

        public async Task<IEnumerable<ExaminationDto>> GetExaminationsByPatientAsync(int patientId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/examinations/patient/{patientId}");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ExaminationDto>>(jsonContent, _jsonOptions) ?? new List<ExaminationDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting examinations for patient {patientId}: {ex.Message}");
                return new List<ExaminationDto>();
            }
        }

        public async Task<ExaminationDto> CreateExaminationAsync(ExaminationDto examination)
        {
            try
            {
                var json = JsonSerializer.Serialize(examination, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/examinations", content);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ExaminationDto>(jsonContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating examination: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Prescriptions

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/prescriptions");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PrescriptionDto>>(jsonContent, _jsonOptions) ?? new List<PrescriptionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting prescriptions: {ex.Message}");
                return new List<PrescriptionDto>();
            }
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByPatientAsync(int patientId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/prescriptions/patient/{patientId}");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PrescriptionDto>>(jsonContent, _jsonOptions) ?? new List<PrescriptionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting prescriptions for patient {patientId}: {ex.Message}");
                return new List<PrescriptionDto>();
            }
        }

        public async Task<PrescriptionDto> CreatePrescriptionAsync(PrescriptionDto prescription)
        {
            try
            {
                var json = JsonSerializer.Serialize(prescription, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/prescriptions", content);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PrescriptionDto>(jsonContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating prescription: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Medical Histories

        public async Task<IEnumerable<MedicalHistoryDto>> GetMedicalHistoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/medicalhistories");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<MedicalHistoryDto>>(jsonContent, _jsonOptions) ?? new List<MedicalHistoryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting medical histories: {ex.Message}");
                return new List<MedicalHistoryDto>();
            }
        }

        public async Task<IEnumerable<MedicalHistoryDto>> GetMedicalHistoriesByPatientAsync(int patientId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/medicalhistories/patient/{patientId}");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<MedicalHistoryDto>>(jsonContent, _jsonOptions) ?? new List<MedicalHistoryDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting medical histories for patient {patientId}: {ex.Message}");
                return new List<MedicalHistoryDto>();
            }
        }

        public async Task<MedicalHistoryDto> CreateMedicalHistoryAsync(MedicalHistoryDto history)
        {
            try
            {
                var json = JsonSerializer.Serialize(history, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/medicalhistories", content);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MedicalHistoryDto>(jsonContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating medical history: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}