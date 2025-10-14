using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Models
{
    public class Nurse : Staff
    {
        public string Shift { get; set; } = "";

        public Nurse() { }
        public override string GetRole() => "Nurse";
    }
}

