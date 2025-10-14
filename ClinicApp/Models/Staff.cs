using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Models
{
    public class Staff : Person
    {
        public string Position { get; set; } = "";
        public int? ClinicId { get; set; }
        public Clinic? Clinic { get; set; }

        public Staff() { }
        public Staff(string firstName, string lastName) : base(firstName, lastName) { }

        public override string GetRole() => "Staff";
    }
}
