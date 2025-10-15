using ClinicApp.Data;
using ClinicApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicApp.Services
{
    public class StaffService
    {
        private readonly ClinicDbContext _context;

        public StaffService(ClinicDbContext context)
        {
            _context = context;
        }

        // LINQ - Svi lekari sortirani
        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return doctors.OrderBy(d => d.LastName).ToList();
        }

        // Dodavanje lekara
        public async Task AddDoctorAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
        }

        // LINQ - Sve sestre sortirane
        public async Task<List<Nurse>> GetAllNursesAsync()
        {
            var nurses = await _context.Nurses.ToListAsync();
            return nurses.OrderBy(n => n.LastName).ToList();
        }

        // Dodavanje sestre
        public async Task AddNurseAsync(Nurse nurse)
        {
            await _context.Nurses.AddAsync(nurse);
            await _context.SaveChangesAsync();
        }
    }
}