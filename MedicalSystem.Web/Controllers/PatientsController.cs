
using Microsoft.AspNetCore.Mvc;
using MedicalSystem.API.DTOs;
using MedicalSystem.Web.Services;

namespace MedicalSystem.Web.Controllers  
{
    public class PatientsController : Controller  
    {
        private readonly IApiService _apiService;

        public PatientsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Patients
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            try
            {
                IEnumerable<PatientDto> patients;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    patients = await _apiService.SearchPatientsAsync(searchTerm);
                    ViewBag.SearchTerm = searchTerm;
                }
                else
                {
                    patients = await _apiService.GetPatientsAsync();
                }

                return View(patients);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška pri dohvaćanju pacijenata: {ex.Message}";
                return View(new List<PatientDto>());
            }
        }

        // GET: /Patients/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var patient = await _apiService.GetPatientAsync(id);
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Pacijent nije pronađen.";
                    return RedirectToAction(nameof(Index));
                }

                return View(patient);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška pri dohvaćanju pacijenta: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Patients/Create
        public IActionResult Create()
        {
            return View(new PatientDto());
        }

        // POST: /Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientDto patient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _apiService.CreatePatientAsync(patient);
                    if (result != null)
                    {
                        TempData["SuccessMessage"] = "Pacijent je uspješno dodan.";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["ErrorMessage"] = "Greška pri dodavanju pacijenta. Molimo pokušajte ponovno.";
                }

                return View(patient);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška pri dodavanju pacijenta: {ex.Message}";
                return View(patient);
            }
        }

        // GET: /Patients/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var patient = await _apiService.GetPatientAsync(id);
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Pacijent nije pronađen.";
                    return RedirectToAction(nameof(Index));
                }

                return View(patient);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška pri dohvaćanju pacijenta: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientDto patient)
        {
            try
            {
                if (id != patient.PatientId)
                {
                    TempData["ErrorMessage"] = "ID pacijenta se ne slaže.";
                    return RedirectToAction(nameof(Index));
                }

                if (ModelState.IsValid)
                {
                    var result = await _apiService.UpdatePatientAsync(id, patient);
                    if (result)
                    {
                        TempData["SuccessMessage"] = "Pacijent je uspješno ažuriran.";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["ErrorMessage"] = "Greška pri ažuriranju pacijenta.";
                }

                return View(patient);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška pri ažuriranju pacijenta: {ex.Message}";
                return View(patient);
            }
        }

        // GET: /Patients/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var patient = await _apiService.GetPatientAsync(id);
                if (patient == null)
                {
                    TempData["ErrorMessage"] = "Pacijent nije pronađen.";
                    return RedirectToAction(nameof(Index));
                }

                return View(patient);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška pri dohvaćanju pacijenta: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _apiService.DeletePatientAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Pacijent je uspješno obrisan.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Greška pri brisanju pacijenta.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška pri brisanju pacijenta: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Patients/Export/5
        public async Task<IActionResult> Export(int id)
        {
            try
            {
                var data = await _apiService.ExportPatientAsync(id);
                if (data == null)
                {
                    TempData["ErrorMessage"] = "Nema podataka za export.";
                    return RedirectToAction(nameof(Index));
                }

                return File(data, "text/csv", $"patient_{id}_data.csv");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška pri exportu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}