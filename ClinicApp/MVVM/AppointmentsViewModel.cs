using ClinicApp.Delegates;
using ClinicApp.Models;
using ClinicApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicApp.ViewModels
{
    public class AppointmentsViewModel
    {
        private readonly AppointmentService _appointmentService;
        private readonly PatientService _patientService;
        private readonly StaffService _staffService;

        public event DataChangedHandler? DataChanged;

        public ObservableCollection<Appointment> Appointments { get; set; } = new();
        public ObservableCollection<Patient> Patients { get; set; } = new();
        public ObservableCollection<Doctor> Doctors { get; set; } = new();

        public Patient? SelectedPatient { get; set; }
        public Doctor? SelectedDoctor { get; set; }
        public DateTime? SelectedDate { get; set; }
        public string AppointmentPurpose { get; set; } = string.Empty;

        public ICommand CreateAppointmentCommand { get; }
        public ICommand RefreshCommand { get; }

        public AppointmentsViewModel(AppointmentService appointmentService, PatientService patientService, StaffService staffService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _staffService = staffService;

            CreateAppointmentCommand = new RelayCommand(async () => await CreateAppointmentAsync());
            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
        }

        protected virtual void OnDataChanged(string message)
        {
            DataChanged?.Invoke(this, message);
        }

        public async Task LoadDataAsync()
        {
            var patients = await _patientService.GetAllAsync();
            var doctors = await _staffService.GetAllDoctorsAsync();
            var appointments = await _appointmentService.GetUpcomingAsync();

            Patients.Clear();
            foreach (var p in patients) Patients.Add(p);

            Doctors.Clear();
            foreach (var d in doctors) Doctors.Add(d);

            Appointments.Clear();
            foreach (var a in appointments) Appointments.Add(a);
        }

        private async Task CreateAppointmentAsync()
        {
            if (SelectedPatient == null || SelectedDoctor == null || !SelectedDate.HasValue)
            {
                OnDataChanged("Sva polja moraju biti popunjena!");
                return;
            }

            var appointment = new Appointment
            {
                PatientId = SelectedPatient.Id,
                DoctorId = SelectedDoctor.Id,
                Start = SelectedDate.Value.AddHours(9),
                DurationMinutes = 30,
                Purpose = AppointmentPurpose.Trim(),
                Status = AppointmentStatus.Scheduled
            };

            await _appointmentService.AddAsync(appointment);
            OnDataChanged("Termin uspešno zakazan!");
            await LoadDataAsync();
            ClearForm();
        }

        private void ClearForm()
        {
            SelectedPatient = null;
            SelectedDoctor = null;
            SelectedDate = null;
            AppointmentPurpose = string.Empty;
        }
    }
}
