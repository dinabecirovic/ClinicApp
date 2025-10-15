using ClinicApp.Data;
using ClinicApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicApp.Services
{
    public class InvoiceService
    {
        private readonly ClinicDbContext _context;

        public InvoiceService(ClinicDbContext context)
        {
            _context = context;
        }

        // Sve fakture
        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        // Dodavanje fakture
        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        // LINQ - Ukupan prihod
        public async Task<decimal> GetTotalRevenueAsync()
        {
            var invoices = await GetAllAsync();
            return invoices.Sum(i => i.Amount);
        }

        // LINQ - Prihod danas
        public async Task<decimal> GetTodayRevenueAsync()
        {
            var invoices = await GetAllAsync();
            return invoices
                .Where(i => i.Date.Date == DateTime.Today)
                .Sum(i => i.Amount);
        }

        // LINQ - Prihod ovog meseca
        public async Task<decimal> GetMonthRevenueAsync()
        {
            var invoices = await GetAllAsync();
            return invoices
                .Where(i => i.Date.Year == DateTime.Now.Year && i.Date.Month == DateTime.Now.Month)
                .Sum(i => i.Amount);
        }

        // LINQ - GroupBy za prihod po uslugama
        public async Task<List<(string Service, decimal Total, int Count)>> GetRevenueByServiceAsync()
        {
            var invoices = await GetAllAsync();

            return invoices
                .GroupBy(i => i.Service)
                .Select(g => (
                    Service: g.Key,
                    Total: g.Sum(i => i.Amount),
                    Count: g.Count()
                ))
                .OrderByDescending(x => x.Total)
                .ToList();
        }
    }
}