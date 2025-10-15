using ClinicApp.Delegates;
using ClinicApp.Models;
using ClinicApp.Services;
using System.Windows;

namespace ClinicApp.Views
{
    public partial class PatientsWindow : Window
    {
        private readonly PatientService _patientService;
        public event DataChangedHandler? DataChanged;

        public PatientsWindow()
        {
            InitializeComponent();
            _patientService = new PatientService(App.DbContext!);
            DataChanged += OnDataChanged;
        }

        private void OnDataChanged(object sender, string message)
        {
            MessageBox.Show(message, "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPatientsAsync();
        }

        private async System.Threading.Tasks.Task LoadPatientsAsync()
        {
            var patients = await _patientService.GetAllAsync();
            PatientsDataGrid.ItemsSource = patients;
        }

        private async void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PatientFirstName.Text) ||
                string.IsNullOrWhiteSpace(PatientLastName.Text) ||
                !PatientBirthDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Sva obavezna polja moraju biti popunjena!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patient = new Patient
            {
                FirstName = PatientFirstName.Text.Trim(),
                LastName = PatientLastName.Text.Trim(),
                BirthDate = PatientBirthDate.SelectedDate.Value,
                RecordNumber = PatientRecordNumber.Text.Trim()
            };

            await _patientService.AddAsync(patient);
            DataChanged?.Invoke(this, "Pacijent uspešno dodat!");
            await LoadPatientsAsync();
            ClearForm();
        }

        private async void DeletePatient_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete pacijenta {selectedPatient.FullName}?",
                    "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _patientService.DeleteAsync(selectedPatient);
                    DataChanged?.Invoke(this, "Pacijent obrisan!");
                    await LoadPatientsAsync();
                }
            }
            else
            {
                MessageBox.Show("Molimo izaberite pacijenta!", "Upozorenje",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadPatientsAsync();
        }

        private void ClearForm()
        {
            PatientFirstName.Clear();
            PatientLastName.Clear();
            PatientBirthDate.SelectedDate = null;
            PatientRecordNumber.Clear();
        }
    }
}