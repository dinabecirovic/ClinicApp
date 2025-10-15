using ClinicApp.Delegates;
using ClinicApp.Models;
using ClinicApp.Services;
using System;
using System.Windows;

namespace ClinicApp.Views
{
    public partial class AppointmentsWindow : Window
    {
        private readonly AppointmentService _appointmentService;
        private readonly PatientService _patientService;
        private readonly StaffService _staffService;
        public event DataChangedHandler? DataChanged;

        public AppointmentsWindow()
        {
            InitializeComponent();
            _appointmentService = new AppointmentService(App.DbContext!);
            _patientService = new PatientService(App.DbContext!);
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
            var patients = await _patientService.GetAllAsync();
            var doctors = await _staffService.GetAllDoctorsAsync();
            var appointments = await _appointmentService.GetUpcomingAsync();

            AppointmentPatient.ItemsSource = patients;
            AppointmentDoctor.ItemsSource = doctors;
            AppointmentsDataGrid.ItemsSource = appointments;
        }

        private async void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentPatient.SelectedItem == null ||
                AppointmentDoctor.SelectedItem == null ||
                !AppointmentDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Sva polja moraju biti popunjena!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patient = (Patient)AppointmentPatient.SelectedItem;
            var doctor = (Doctor)AppointmentDoctor.SelectedItem;

            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                Start = AppointmentDate.SelectedDate.Value.AddHours(9),
                DurationMinutes = 30,
                Purpose = AppointmentPurpose.Text.Trim(),
                Status = AppointmentStatus.Scheduled
            };

            await _appointmentService.AddAsync(appointment);
            DataChanged?.Invoke(this, "Termin uspešno zakazan!");
            await LoadDataAsync();
            ClearForm();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private void ClearForm()
        {
            AppointmentPatient.SelectedItem = null;
            AppointmentDoctor.SelectedItem = null;
            AppointmentDate.SelectedDate = null;
            AppointmentPurpose.Clear();
        }
    }
}