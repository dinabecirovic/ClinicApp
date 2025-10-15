using ClinicApp.Delegates;
using ClinicApp.Models;
using ClinicApp.Services;
using System.Windows;

namespace ClinicApp.Views
{
    public partial class StaffWindow : Window
    {
        private readonly StaffService _staffService;
        public event DataChangedHandler? DataChanged;

        public StaffWindow()
        {
            InitializeComponent();
            _staffService = new StaffService(App.DbContext!);
            DataChanged += OnDataChanged;
        }

        private void OnDataChanged(object sender, string message)
        {
            MessageBox.Show(message, "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            var doctors = await _staffService.GetAllDoctorsAsync();
            var nurses = await _staffService.GetAllNursesAsync();

            DoctorsDataGrid.ItemsSource = doctors;
            NursesDataGrid.ItemsSource = nurses;
        }

        private async void AddDoctor_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DoctorFirstName.Text) ||
                string.IsNullOrWhiteSpace(DoctorLastName.Text) ||
                string.IsNullOrWhiteSpace(DoctorSpecialty.Text))
            {
                MessageBox.Show("Sva polja su obavezna!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var doctor = new Doctor
            {
                FirstName = DoctorFirstName.Text.Trim(),
                LastName = DoctorLastName.Text.Trim(),
                Specialty = DoctorSpecialty.Text.Trim(),
                Position = "Lekar"
            };

            await _staffService.AddDoctorAsync(doctor);
            DataChanged?.Invoke(this, "Lekar dodat!");
            await LoadDataAsync();

            DoctorFirstName.Clear();
            DoctorLastName.Clear();
            DoctorSpecialty.Clear();
        }

        private async void AddNurse_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NurseFirstName.Text) ||
                string.IsNullOrWhiteSpace(NurseLastName.Text) ||
                string.IsNullOrWhiteSpace(NurseShift.Text))
            {
                MessageBox.Show("Sva polja su obavezna!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nurse = new Nurse
            {
                FirstName = NurseFirstName.Text.Trim(),
                LastName = NurseLastName.Text.Trim(),
                Shift = NurseShift.Text.Trim(),
                Position = "Medicinska sestra"
            };

            await _staffService.AddNurseAsync(nurse);
            DataChanged?.Invoke(this, "Medicinska sestra dodata!");
            await LoadDataAsync();

            NurseFirstName.Clear();
            NurseLastName.Clear();
            NurseShift.Clear();
        }

        private async void RefreshDoctors_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async void RefreshNurses_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }
    }
}