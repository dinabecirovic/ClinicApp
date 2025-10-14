using ClinicApp.Data;
using ClinicApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClinicApp
{
    public partial class MainWindow : Window
    {
        private readonly ClinicDbContext _context;

        // DELEGAT - Definicija delegata
        public delegate void DataChangedHandler(string message);

        // EVENT - Definicija eventa
        public event DataChangedHandler? DataChanged;

        public MainWindow()
        {
            InitializeComponent();
            _context = App.DbContext!;

            // Pretplata na event
            DataChanged += OnDataChangedNotification;
        }

        // Event handler za notifikacije
        private void OnDataChangedNotification(string message)
        {
            MessageBox.Show(message, "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Poziva event
        private void RaiseDataChanged(string message)
        {
            DataChanged?.Invoke(message);
        }

        // Učitavanje podataka pri pokretanju
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllDataAsync();
        }

        // Asinhrono učitavanje svih podataka
        private async Task LoadAllDataAsync()
        {
            await LoadPatientsAsync();
            await LoadAppointmentsAsync();
            await LoadDoctorsAsync();
            await LoadNursesAsync();
            await LoadInvoicesAsync();
        }

        #region PACIJENTI

        // Asinhrono učitavanje pacijenata
        private async Task LoadPatientsAsync()
        {
            var patients = await _context.Patients.ToListAsync();

            // LINQ - Sortiranje
            var sortedPatients = patients.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToList();

            PatientsDataGrid.ItemsSource = sortedPatients;
            AppointmentPatient.ItemsSource = sortedPatients;
            RecordPatientSelector.ItemsSource = sortedPatients;
            InvoicePatient.ItemsSource = sortedPatients;
        }

        // Dodavanje pacijenta
        private async void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            // Validacija
            if (string.IsNullOrWhiteSpace(PatientFirstName.Text))
            {
                MessageBox.Show("Ime je obavezno!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PatientLastName.Text))
            {
                MessageBox.Show("Prezime je obavezno!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!PatientBirthDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Datum rođenja je obavezan!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kreiranje novog pacijenta
            var patient = new Patient
            {
                FirstName = PatientFirstName.Text.Trim(),
                LastName = PatientLastName.Text.Trim(),
                BirthDate = PatientBirthDate.SelectedDate.Value,
                RecordNumber = PatientRecordNumber.Text.Trim()
            };

            // Asinhrono dodavanje u bazu
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            // Pozivanje eventa
            RaiseDataChanged("Pacijent uspešno dodat!");

            // Refresh
            await LoadPatientsAsync();

            // Čišćenje forme
            ClearPatientForm();
        }

        // Brisanje pacijenta
        private async void DeletePatient_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete pacijenta {selectedPatient.FullName}?",
                    "Potvrda brisanja",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Patients.Remove(selectedPatient);
                    await _context.SaveChangesAsync();

                    RaiseDataChanged("Pacijent obrisan!");
                    await LoadPatientsAsync();
                }
            }
            else
            {
                MessageBox.Show("Molimo izaberite pacijenta!", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearPatientForm()
        {
            PatientFirstName.Clear();
            PatientLastName.Clear();
            PatientBirthDate.SelectedDate = null;
            PatientRecordNumber.Clear();
        }

        #endregion

        #region TERMINI

        // Asinhrono učitavanje termina
        private async Task LoadAppointmentsAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();

            // LINQ - Filtriranje i sortiranje
            var upcomingAppointments = appointments
                .Where(a => a.Start >= DateTime.Now)
                .OrderBy(a => a.Start)
                .ToList();

            AppointmentsDataGrid.ItemsSource = upcomingAppointments;
        }

        // Zakazivanje termina
        private async void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentPatient.SelectedItem == null)
            {
                MessageBox.Show("Izaberite pacijenta!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AppointmentDoctor.SelectedItem == null)
            {
                MessageBox.Show("Izaberite lekara!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!AppointmentDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Izaberite datum!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patient = (Patient)AppointmentPatient.SelectedItem;
            var doctor = (Doctor)AppointmentDoctor.SelectedItem;

            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                Start = AppointmentDate.SelectedDate.Value.AddHours(9), // Postavlja na 9h
                DurationMinutes = 30,
                Purpose = AppointmentPurpose.Text.Trim(),
                Status = AppointmentStatus.Scheduled
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            RaiseDataChanged("Termin uspešno zakazan!");
            await LoadAppointmentsAsync();

            ClearAppointmentForm();
        }

        private void ClearAppointmentForm()
        {
            AppointmentPatient.SelectedItem = null;
            AppointmentDoctor.SelectedItem = null;
            AppointmentDate.SelectedDate = null;
            AppointmentPurpose.Clear();
        }

        #endregion

        #region MEDICINSKI KARTONI

        // Promena pacijenta
        private async void RecordPatient_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (RecordPatientSelector.SelectedItem is Patient selectedPatient)
            {
                var records = await _context.MedicalRecords
                    .Where(r => r.PatientId == selectedPatient.Id)
                    .OrderByDescending(r => r.Date)
                    .ToListAsync();

                RecordsDataGrid.ItemsSource = records;
            }
        }

        // Dodavanje medicinskog kartona
        private async void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            if (RecordPatientSelector.SelectedItem == null)
            {
                MessageBox.Show("Izaberite pacijenta!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(RecordDiagnosis.Text))
            {
                MessageBox.Show("Dijagnoza je obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patient = (Patient)RecordPatientSelector.SelectedItem;

            var record = new MedicalRecord
            {
                PatientId = patient.Id,
                Date = DateTime.Now,
                Diagnosis = RecordDiagnosis.Text.Trim(),
                Treatment = RecordTreatment.Text.Trim(),
                Notes = RecordNotes.Text.Trim()
            };

            await _context.MedicalRecords.AddAsync(record);
            await _context.SaveChangesAsync();

            RaiseDataChanged("Medicinski karton dodat!");

            // Refresh kartona za trenutnog pacijenta
            var records = await _context.MedicalRecords
                .Where(r => r.PatientId == patient.Id)
                .OrderByDescending(r => r.Date)
                .ToListAsync();

            RecordsDataGrid.ItemsSource = records;

            ClearRecordForm();
        }

        private void ClearRecordForm()
        {
            RecordDiagnosis.Clear();
            RecordTreatment.Clear();
            RecordNotes.Clear();
        }

        #endregion

        #region OSOBLJE

        // Učitavanje lekara
        private async Task LoadDoctorsAsync()
        {
            var doctors = await _context.Doctors.ToListAsync();

            // LINQ - Sortiranje
            var sortedDoctors = doctors.OrderBy(d => d.LastName).ToList();

            DoctorsDataGrid.ItemsSource = sortedDoctors;
            AppointmentDoctor.ItemsSource = sortedDoctors;
        }

        // Dodavanje lekara
        private async void AddDoctor_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DoctorFirstName.Text) ||
                string.IsNullOrWhiteSpace(DoctorLastName.Text) ||
                string.IsNullOrWhiteSpace(DoctorSpecialty.Text))
            {
                MessageBox.Show("Sva polja su obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var doctor = new Doctor
            {
                FirstName = DoctorFirstName.Text.Trim(),
                LastName = DoctorLastName.Text.Trim(),
                Specialty = DoctorSpecialty.Text.Trim(),
                Position = "Lekar"
            };

            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            RaiseDataChanged("Lekar dodat!");
            await LoadDoctorsAsync();

            DoctorFirstName.Clear();
            DoctorLastName.Clear();
            DoctorSpecialty.Clear();
        }

        // Učitavanje sestara
        private async Task LoadNursesAsync()
        {
            var nurses = await _context.Nurses.ToListAsync();

            // LINQ - Sortiranje
            var sortedNurses = nurses.OrderBy(n => n.LastName).ToList();

            NursesDataGrid.ItemsSource = sortedNurses;
        }

        // Dodavanje sestre
        private async void AddNurse_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NurseFirstName.Text) ||
                string.IsNullOrWhiteSpace(NurseLastName.Text) ||
                string.IsNullOrWhiteSpace(NurseShift.Text))
            {
                MessageBox.Show("Sva polja su obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nurse = new Nurse
            {
                FirstName = NurseFirstName.Text.Trim(),
                LastName = NurseLastName.Text.Trim(),
                Shift = NurseShift.Text.Trim(),
                Position = "Medicinska sestra"
            };

            await _context.Nurses.AddAsync(nurse);
            await _context.SaveChangesAsync();

            RaiseDataChanged("Medicinska sestra dodata!");
            await LoadNursesAsync();

            NurseFirstName.Clear();
            NurseLastName.Clear();
            NurseShift.Clear();
        }

        #endregion
        #region FAKTURISANJE I IZVEŠTAJI

        // Učitavanje faktura
        private async Task LoadInvoicesAsync()
        {
            var invoices = await _context.Invoices
                .OrderByDescending(i => i.Date)
                .ToListAsync();

            InvoicesDataGrid.ItemsSource = invoices;

            // LINQ - Aggregation
            var total = invoices.Sum(i => i.Amount);
            TotalRevenue.Text = $"Ukupan prihod: {total:N2} RSD";
        }

        // Kreiranje fakture
        private async void CreateInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicePatient.SelectedItem == null)
            {
                MessageBox.Show("Izaberite pacijenta!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(InvoiceService.Text))
            {
                MessageBox.Show("Unesite uslugu!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(InvoiceAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Unesite validnu cenu!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patient = (Patient)InvoicePatient.SelectedItem;

            var invoice = new Invoice
            {
                Date = DateTime.Now,
                PatientName = patient.FullName,
                Service = InvoiceService.Text.Trim(),
                Amount = amount
            };

            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            RaiseDataChanged($"Faktura kreirana za pacijenta {patient.FullName}!");
            await LoadInvoicesAsync();

            InvoicePatient.SelectedItem = null;
            InvoiceService.Text = "Pregled";
            InvoiceAmount.Text = "3000";
        }

        // Izveštaj - Broj pacijenata
        private async void ReportPatients_Click(object sender, RoutedEventArgs e)
        {
            var patients = await _context.Patients.ToListAsync();

            // LINQ - Count, GroupBy
            var totalPatients = patients.Count;
            var maleCount = patients.Count(p => p.FirstName.EndsWith("a") == false); // Aproksimacija
            var femaleCount = patients.Count(p => p.FirstName.EndsWith("a"));

            var report = "=== IZVEŠTAJ O PACIJENTIMA ===\n\n";
            report += $"Ukupan broj pacijenata: {totalPatients}\n";
            report += $"Približno muških: {maleCount}\n";
            report += $"Približno ženskih: {femaleCount}\n\n";
            report += "Top 10 pacijenata po broju termina:\n";

            // LINQ - Complex query
            var patientAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .GroupBy(a => a.Patient)
                .Select(g => new { Patient = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            foreach (var pa in patientAppointments)
            {
                if (pa.Patient != null)
                {
                    report += $"  - {pa.Patient.FullName}: {pa.Count} termina\n";
                }
            }

            ReportOutput.Text = report;
        }

        // Izveštaj - Broj termina
        private async void ReportAppointments_Click(object sender, RoutedEventArgs e)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();

            // LINQ - Filtering and Grouping
            var total = appointments.Count;
            var scheduled = appointments.Count(a => a.Status == AppointmentStatus.Scheduled);
            var completed = appointments.Count(a => a.Status == AppointmentStatus.Completed);
            var canceled = appointments.Count(a => a.Status == AppointmentStatus.Canceled);

            var report = "=== IZVEŠTAJ O TERMINIMA ===\n\n";
            report += $"Ukupno termina: {total}\n";
            report += $"Zakazani: {scheduled}\n";
            report += $"Završeni: {completed}\n";
            report += $"Otkazani: {canceled}\n\n";

            report += "Predstojеći termini (sledeća 7 dana):\n";

            // LINQ - Complex filtering
            var upcoming = appointments
                .Where(a => a.Start >= DateTime.Now && a.Start <= DateTime.Now.AddDays(7))
                .OrderBy(a => a.Start)
                .ToList();

            foreach (var apt in upcoming)
            {
                report += $"  - {apt.Start:dd.MM.yyyy HH:mm} | {apt.Patient?.FullName} | Dr. {apt.Doctor?.LastName}\n";
            }

            if (upcoming.Count == 0)
            {
                report += "  (Nema predstojećih termina)\n";
            }

            ReportOutput.Text = report;
        }

        // Izveštaj - Prihod
        private async void ReportRevenue_Click(object sender, RoutedEventArgs e)
        {
            var invoices = await _context.Invoices.ToListAsync();

            // LINQ - Aggregation and Grouping
            var totalRevenue = invoices.Sum(i => i.Amount);
            var todayRevenue = invoices
                .Where(i => i.Date.Date == DateTime.Today)
                .Sum(i => i.Amount);

            var thisMonthRevenue = invoices
                .Where(i => i.Date.Year == DateTime.Now.Year && i.Date.Month == DateTime.Now.Month)
                .Sum(i => i.Amount);

            var report = "=== IZVEŠTAJ O PRIHODU ===\n\n";
            report += $"Ukupan prihod: {totalRevenue:N2} RSD\n";
            report += $"Prihod danas: {todayRevenue:N2} RSD\n";
            report += $"Prihod ovog meseca: {thisMonthRevenue:N2} RSD\n\n";

            // LINQ - GroupBy
            var revenueByService = invoices
                .GroupBy(i => i.Service)
                .Select(g => new { Service = g.Key, Total = g.Sum(i => i.Amount), Count = g.Count() })
                .OrderByDescending(x => x.Total)
                .ToList();

            report += "Prihod po uslugama:\n";
            foreach (var service in revenueByService)
            {
                report += $"  - {service.Service}: {service.Total:N2} RSD ({service.Count} faktura)\n";
            }

            ReportOutput.Text = report;
        }

        #endregion
    }

}