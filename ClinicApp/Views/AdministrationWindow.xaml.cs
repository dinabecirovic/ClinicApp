using ClinicApp.Services;
using ClinicApp.ViewModels;
using System.Windows;

namespace ClinicApp.Views
{
    public partial class AdministrationWindow : Window
    {
        private readonly AdministrationViewModel _viewModel;

        public AdministrationWindow()
        {
            InitializeComponent();

            // Kreiranje servisa
            var invoiceService = new InvoiceService(App.DbContext!);
            var patientService = new PatientService(App.DbContext!);
            var appointmentService = new AppointmentService(App.DbContext!);

            // Kreiranje ViewModel-a
            _viewModel = new AdministrationViewModel(invoiceService, patientService, appointmentService);

            // Subscribovanje na event za obaveštenje
            _viewModel.DataChanged += (s, message) =>
            {
                MessageBox.Show(message, "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            // Povezivanje sa XAML
            DataContext = _viewModel;

            // Učitavanje podataka kada se prozor otvori
            Loaded += async (s, e) =>
            {
                await _viewModel.LoadPatientsAsync();
                await _viewModel.LoadInvoicesAsync();
            };
        }
    }
}
