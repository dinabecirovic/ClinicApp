using ClinicApp.Data;
using ClinicApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicApp.Services
{
    public class PatientService
    {
        private readonly ClinicDbContext _context;

        public PatientService(ClinicDbContext context)
        {
            _context = context;
        }

        // Asinhrono učitavanje svih pacijenata
        public async Task<List<Patient>> GetAllAsync()
        {
            var patients = await _context.Patients.ToListAsync();

            // LINQ - Sortiranje
            return patients.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToList();
        }

        // Dodavanje pacijenta
        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        // Brisanje pacijenta
        public async Task DeleteAsync(Patient patient)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }

        // LINQ - Pretraga pacijenata
        public async Task<List<Patient>> SearchAsync(string searchTerm)
        {
            return await _context.Patients
                .Where(p => p.FirstName.Contains(searchTerm) ||
                           p.LastName.Contains(searchTerm) ||
                           p.RecordNumber.Contains(searchTerm))
                .OrderBy(p => p.LastName)
                .ToListAsync();
        }
    }
}