using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Models
{
    public enum AppointmentStatus { Scheduled, Completed, Canceled }

    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public DateTime Start { get; set; }
        public int DurationMinutes { get; set; } = 30;
        public string Purpose { get; set; } = "";
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    }
}

