using ClinicApp.Models;
using ClinicApp.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClinicApp.ViewModels
{
    public class StaffViewModel : INotifyPropertyChanged
    {
        private readonly StaffService _staffService;

        // Doctor Properties
        private ObservableCollection<Doctor> _doctors = new();
        public ObservableCollection<Doctor> Doctors
        {
            get => _doctors;
            set
            {
                _doctors = value;
                OnPropertyChanged();
            }
        }

        private string _doctorFirstName = string.Empty;
        public string DoctorFirstName
        {
            get => _doctorFirstName;
            set
            {
                _doctorFirstName = value;
                OnPropertyChanged();
                ((RelayCommand)AddDoctorCommand).RaiseCanExecuteChanged();
            }
        }

        private string _doctorLastName = string.Empty;
        public string DoctorLastName
        {
            get => _doctorLastName;
            set
            {
                _doctorLastName = value;
                OnPropertyChanged();
                ((RelayCommand)AddDoctorCommand).RaiseCanExecuteChanged();
            }
        }

        private string _doctorSpecialty = string.Empty;
        public string DoctorSpecialty
        {
            get => _doctorSpecialty;
            set
            {
                _doctorSpecialty = value;
                OnPropertyChanged();
                ((RelayCommand)AddDoctorCommand).RaiseCanExecuteChanged();
            }
        }

        // Nurse Properties
        private ObservableCollection<Nurse> _nurses = new();
        public ObservableCollection<Nurse> Nurses
        {
            get => _nurses;
            set
            {
                _nurses = value;
                OnPropertyChanged();
            }
        }

        private string _nurseFirstName = string.Empty;
        public string NurseFirstName
        {
            get => _nurseFirstName;
            set
            {
                _nurseFirstName = value;
                OnPropertyChanged();
                ((RelayCommand)AddNurseCommand).RaiseCanExecuteChanged();
            }
        }

        private string _nurseLastName = string.Empty;
        public string NurseLastName
        {
            get => _nurseLastName;
            set
            {
                _nurseLastName = value;
                OnPropertyChanged();
                ((RelayCommand)AddNurseCommand).RaiseCanExecuteChanged();
            }
        }

        private string _nurseShift = string.Empty;
        public string NurseShift
        {
            get => _nurseShift;
            set
            {
                _nurseShift = value;
                OnPropertyChanged();
                ((RelayCommand)AddNurseCommand).RaiseCanExecuteChanged();
            }
        }

        // Commands
        public ICommand AddDoctorCommand { get; }
        public ICommand AddNurseCommand { get; }
        public ICommand RefreshDoctorsCommand { get; }
        public ICommand RefreshNursesCommand { get; }

        // Constructor
        public StaffViewModel(StaffService staffService)
        {
            _staffService = staffService;

            AddDoctorCommand = new RelayCommand(AddDoctorAsync, CanAddDoctor);
            AddNurseCommand = new RelayCommand(AddNurseAsync, CanAddNurse);
            RefreshDoctorsCommand = new RelayCommand(LoadDataAsync);
            RefreshNursesCommand = new RelayCommand(LoadDataAsync);
        }

        // Methods
        public async Task LoadDataAsync()
        {
            var doctors = await _staffService.GetAllDoctorsAsync();
            var nurses = await _staffService.GetAllNursesAsync();

            Doctors = new ObservableCollection<Doctor>(doctors);
            Nurses = new ObservableCollection<Nurse>(nurses);
        }

        private bool CanAddDoctor()
        {
            return !string.IsNullOrWhiteSpace(DoctorFirstName) &&
                   !string.IsNullOrWhiteSpace(DoctorLastName) &&
                   !string.IsNullOrWhiteSpace(DoctorSpecialty);
        }

        private async Task AddDoctorAsync()
        {
            if (!CanAddDoctor())
            {
                MessageBox.Show("Sva polja su obavezna!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var doctor = new Doctor
            {
                FirstName = DoctorFirstName.Trim(),
                LastName = DoctorLastName.Trim(),
                Specialty = DoctorSpecialty.Trim(),
                Position = "Lekar"
            };

            await _staffService.AddDoctorAsync(doctor);
            MessageBox.Show("Lekar dodat!", "Obaveštenje",
                MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadDataAsync();
            ClearDoctorForm();
        }

        private bool CanAddNurse()
        {
            return !string.IsNullOrWhiteSpace(NurseFirstName) &&
                   !string.IsNullOrWhiteSpace(NurseLastName) &&
                   !string.IsNullOrWhiteSpace(NurseShift);
        }

        private async Task AddNurseAsync()
        {
            if (!CanAddNurse())
            {
                MessageBox.Show("Sva polja su obavezna!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nurse = new Nurse
            {
                FirstName = NurseFirstName.Trim(),
                LastName = NurseLastName.Trim(),
                Shift = NurseShift.Trim(),
                Position = "Medicinska sestra"
            };

            await _staffService.AddNurseAsync(nurse);
            MessageBox.Show("Medicinska sestra dodata!", "Obaveštenje",
                MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadDataAsync();
            ClearNurseForm();
        }

        private void ClearDoctorForm()
        {
            DoctorFirstName = string.Empty;
            DoctorLastName = string.Empty;
            DoctorSpecialty = string.Empty;
        }

        private void ClearNurseForm()
        {
            NurseFirstName = string.Empty;
            NurseLastName = string.Empty;
            NurseShift = string.Empty;
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}