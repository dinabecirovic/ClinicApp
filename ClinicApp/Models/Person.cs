using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Models
{
    public abstract class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName => $"{FirstName} {LastName}";

        protected Person() { }
        protected Person(string firstName, string lastName) { FirstName = firstName; LastName = lastName; }

        public virtual string GetRole() => "Person";
    }
}
