using ClinicApp.Data;
using ClinicApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicApp.Services
{
    public class AppointmentService
    {
        private readonly ClinicDbContext _context;

        public AppointmentService(ClinicDbContext context)
        {
            _context = context;
        }

        // Asinhrono učitavanje termina
        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderBy(a => a.Start)
                .ToListAsync();
        }

        // LINQ - Samo predstojеći termini
        public async Task<List<Appointment>> GetUpcomingAsync()
        {
            var appointments = await GetAllAsync();

            return appointments
                .Where(a => a.Start >= DateTime.Now)
                .OrderBy(a => a.Start)
                .ToList();
        }

        // Dodavanje termina
        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        // LINQ - Termini za narednih 7 dana
        public async Task<List<Appointment>> GetNextWeekAsync()
        {
            var appointments = await GetAllAsync();

            return appointments
                .Where(a => a.Start >= DateTime.Now && a.Start <= DateTime.Now.AddDays(7))
                .OrderBy(a => a.Start)
                .ToList();
        }
    }
}