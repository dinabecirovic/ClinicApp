using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Diagnosis { get; set; } = "";
        public string Treatment { get; set; } = "";
        public string Notes { get; set; } = "";
    }
}

