using System;

namespace ClinicApp.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string PatientName { get; set; } = "";
        public string Service { get; set; } = "";
        public decimal Amount { get; set; }
    }
}