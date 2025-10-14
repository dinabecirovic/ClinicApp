using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Models
{
    public class Patient : Person
    {
        public DateTime BirthDate { get; set; }
        public string RecordNumber { get; set; } = "";
        public List<MedicalRecord> MedicalRecords { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();

        public Patient() { }
        public Patient(string firstName, string lastName) : base(firstName, lastName) { }

        public override string GetRole() => "Patient";
    }
}
