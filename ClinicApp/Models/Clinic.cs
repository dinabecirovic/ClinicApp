using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Models
{
    public class Clinic
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public List<Staff> Employees { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
    }
}

