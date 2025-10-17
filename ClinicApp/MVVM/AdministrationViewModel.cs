using ClinicApp.Delegates;
using ClinicApp.Models;
using ClinicApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicApp.ViewModels
{
    public class AdministrationViewModel
    {
        private readonly InvoiceService _invoiceService;
        private readonly PatientService _patientService;
        private readonly AppointmentService _appointmentService;

        public event DataChangedHandler? DataChanged;

        // Kolekcije
        public ObservableCollection<Invoice> Invoices { get; set; } = new();
        public ObservableCollection<Patient> Patients { get; set; } = new();

        // Odabrani i unos polja
        public Patient? SelectedPatient { get; set; }
        public string InvoiceService { get; set; } = string.Empty;
        public string InvoiceAmount { get; set; } = string.Empty;

        public string TotalRevenue { get; set; } = string.Empty;

        // Komande
        public ICommand CreateInvoiceCommand { get; }

        public AdministrationViewModel(InvoiceService invoiceService, PatientService patientService, AppointmentService appointmentService)
        {
            _invoiceService = invoiceService;
            _patientService = patientService;
            _appointmentService = appointmentService;

            CreateInvoiceCommand = new RelayCommand(async () => await CreateInvoiceAsync());
        }

        protected virtual void OnDataChanged(string message)
        {
            DataChanged?.Invoke(this, message);
        }

        // Učitavanje pacijenata
        public async Task LoadPatientsAsync()
        {
            var patients = await _patientService.GetAllAsync();
            Patients.Clear();
            foreach (var p in patients)
                Patients.Add(p);
        }

        // Učitavanje faktura
        public async Task LoadInvoicesAsync()
        {
            var invoices = await _invoiceService.GetAllAsync();
            Invoices.Clear();
            foreach (var i in invoices)
                Invoices.Add(i);

            var total = await _invoiceService.GetTotalRevenueAsync();
            TotalRevenue = $"Ukupan prihod: {total:N2} RSD";
        }

        // Kreiranje fakture
        private async Task CreateInvoiceAsync()
        {
            if (SelectedPatient == null ||
                string.IsNullOrWhiteSpace(InvoiceService) ||
                !decimal.TryParse(InvoiceAmount, out decimal amount) || amount <= 0)
            {
                OnDataChanged("Sva polja moraju biti validno popunjena!");
                return;
            }

            var invoice = new Invoice
            {
                Date = DateTime.Now,
                PatientName = SelectedPatient.FullName,
                Service = InvoiceService.Trim(),
                Amount = amount
            };

            await _invoiceService.AddAsync(invoice);
            OnDataChanged($"Faktura kreirana za pacijenta {SelectedPatient.FullName}!");
            await LoadInvoicesAsync();

            // Reset
            SelectedPatient = null;
            InvoiceService = string.Empty;
            InvoiceAmount = string.Empty;
        }
    }
}
