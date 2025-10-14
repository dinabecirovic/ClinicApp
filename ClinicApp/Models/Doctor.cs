using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Models
{
    public class Doctor : Staff
    {
        public string Specialty { get; set; } = "";

        public Doctor() { }
        public Doctor(string firstName, string lastName, string specialty) : base(firstName, lastName)
        {
            Specialty = specialty;
        }

        public override string GetRole() => "Doctor";
    }
}

