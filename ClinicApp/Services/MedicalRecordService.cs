using ClinicApp.Data;
using ClinicApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicApp.Services
{
    public class MedicalRecordService
    {
        private readonly ClinicDbContext _context;

        public MedicalRecordService(ClinicDbContext context)
        {
            _context = context;
        }

        // LINQ - Kartoni za određenog pacijenta
        public async Task<List<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return await _context.MedicalRecords
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        // Dodavanje kartona
        public async Task AddAsync(MedicalRecord record)
        {
            await _context.MedicalRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        // Ažuriranje kartona
        public async Task UpdateAsync(MedicalRecord record)
        {
            _context.MedicalRecords.Update(record);
            await _context.SaveChangesAsync();
        }
    }
}