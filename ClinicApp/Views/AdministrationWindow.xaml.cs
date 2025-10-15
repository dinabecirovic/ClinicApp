using ClinicApp.Delegates;
using ClinicApp.Models;
using ClinicApp.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace ClinicApp.Views
{
    public partial class AdministrationWindow : Window
    {
        private readonly InvoiceService _invoiceService;
        private readonly PatientService _patientService;
        private readonly AppointmentService _appointmentService;
        public event DataChangedHandler? DataChanged;

        public AdministrationWindow()
        {
            InitializeComponent();
            _invoiceService = new InvoiceService(App.DbContext!);
            _patientService = new PatientService(App.DbContext!);
            _appointmentService = new AppointmentService(App.DbContext!);
            DataChanged += OnDataChanged;
        }

        private void OnDataChanged(object sender, string message)
        {
            MessageBox.Show(message, "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var patients = await _patientService.GetAllAsync();
            InvoicePatient.ItemsSource = patients;
            await LoadInvoicesAsync();
        }

        private async System.Threading.Tasks.Task LoadInvoicesAsync()
        {
            var invoices = await _invoiceService.GetAllAsync();
            InvoicesDataGrid.ItemsSource = invoices;

            var total = await _invoiceService.GetTotalRevenueAsync();
            TotalRevenue.Text = $"Ukupan prihod: {total:N2} RSD";
        }

        private async void CreateInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (InvoicePatient.SelectedItem == null ||
                string.IsNullOrWhiteSpace(InvoiceService.Text) ||
                !decimal.TryParse(InvoiceAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Sva polja moraju biti validno popunjena!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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

            await _invoiceService.AddAsync(invoice);
            DataChanged?.Invoke(this, $"Faktura kreirana za pacijenta {patient.FullName}!");
            await LoadInvoicesAsync();

            InvoicePatient.SelectedItem = null;
            InvoiceService.Text = "Pregled";
            InvoiceAmount.Text = "3000";
        }

        private async void ReportPatients_Click(object sender, RoutedEventArgs e)
        {
            var patients = await _patientService.GetAllAsync();
            var appointments = await App.DbContext!.Appointments
                .Include(a => a.Patient)
                .ToListAsync();

            var report = "=== IZVEŠTAJ O PACIJENTIMA ===\n\n";
            report += $"Ukupan broj pacijenata: {patients.Count}\n\n";
            report += "Top 10 pacijenata po broju termina:\n";

            // LINQ - GroupBy
            var patientAppointments = appointments
                .GroupBy(a => a.Patient)
                .Select(g => new { Patient = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            foreach (var pa in patientAppointments)
            {
                if (pa.Patient != null)
                {
                    report += $"  - {pa.Patient.FullName}: {pa.Count} termina\n";
                }
            }

            ReportOutput.Text = report;
        }

        private async void ReportAppointments_Click(object sender, RoutedEventArgs e)
        {
            var all = await _appointmentService.GetAllAsync();
            var upcoming = await _appointmentService.GetNextWeekAsync();

            var report = "=== IZVEŠTAJ O TERMINIMA ===\n\n";
            report += $"Ukupno termina: {all.Count}\n";
            report += $"Zakazani: {all.Count(a => a.Status == AppointmentStatus.Scheduled)}\n";
            report += $"Završeni: {all.Count(a => a.Status == AppointmentStatus.Completed)}\n";
            report += $"Otkazani: {all.Count(a => a.Status == AppointmentStatus.Canceled)}\n\n";
            report += "Predstojеći termini (sledeća 7 dana):\n";

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

        private async void ReportRevenue_Click(object sender, RoutedEventArgs e)
        {
            var total = await _invoiceService.GetTotalRevenueAsync();
            var today = await _invoiceService.GetTodayRevenueAsync();
            var month = await _invoiceService.GetMonthRevenueAsync();
            var byService = await _invoiceService.GetRevenueByServiceAsync();

            var report = "IZVEŠTAJ O PRIHODU\n";
            report += $"Ukupan prihod: {total:N2} RSD\n";
            report += $"Prihod danas: {today:N2} RSD\n";
            report += $"Prihod ovog meseca: {month:N2} RSD\n\n";
            report += "Prihod po uslugama:\n";

            foreach (var service in byService)
            {
                report += $"  - {service.Service}: {service.Total:N2} RSD ({service.Count} faktura)\n";
            }

            ReportOutput.Text = report;
        }
    }
}
