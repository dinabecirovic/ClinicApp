using ClinicApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Data
{
    public class ClinicDbContext : DbContext
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Staff> StaffMembers => Set<Staff>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Nurse> Nurses => Set<Nurse>();
        public DbSet<Clinic> Clinics => Set<Clinic>();
        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<Staff>()
                .HasDiscriminator<string>("StaffType")
                .HasValue<Staff>("Staff")
                .HasValue<Doctor>("Doctor")
                .HasValue<Nurse>("Nurse");

            base.OnModelCreating(modelBuilder);
        }
    }
}
