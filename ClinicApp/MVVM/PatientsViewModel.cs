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
    public class PatientsViewModel : INotifyPropertyChanged
    {
        private readonly PatientService _patientService;

        // Properties
        private ObservableCollection<Patient> _patients = new();
        public ObservableCollection<Patient> Patients
        {
            get => _patients;
            set
            {
                _patients = value;
                OnPropertyChanged();
            }
        }

        private Patient? _selectedPatient;
        public Patient? SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                _selectedPatient = value;
                OnPropertyChanged();
                ((RelayCommand)DeletePatientCommand).RaiseCanExecuteChanged();
            }
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
                ((RelayCommand)AddPatientCommand).RaiseCanExecuteChanged();
            }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
                ((RelayCommand)AddPatientCommand).RaiseCanExecuteChanged();
            }
        }

        private DateTime? _birthDate;
        public DateTime? BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                OnPropertyChanged();
                ((RelayCommand)AddPatientCommand).RaiseCanExecuteChanged();
            }
        }

        private string _recordNumber = string.Empty;
        public string RecordNumber
        {
            get => _recordNumber;
            set
            {
                _recordNumber = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand AddPatientCommand { get; }
        public ICommand DeletePatientCommand { get; }
        public ICommand RefreshCommand { get; }

        // Constructor
        public PatientsViewModel(PatientService patientService)
        {
            _patientService = patientService;

            AddPatientCommand = new RelayCommand(AddPatientAsync, CanAddPatient);
            DeletePatientCommand = new RelayCommand(DeletePatientAsync, CanDeletePatient);
            RefreshCommand = new RelayCommand(LoadPatientsAsync);
        }

        // Methods
        public async Task LoadPatientsAsync()
        {
            var patients = await _patientService.GetAllAsync();
            Patients = new ObservableCollection<Patient>(patients);
        }

        private bool CanAddPatient()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   BirthDate.HasValue;
        }

        private async Task AddPatientAsync()
        {
            if (!CanAddPatient())
            {
                MessageBox.Show("Sva obavezna polja moraju biti popunjena!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patient = new Patient
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                BirthDate = BirthDate!.Value,
                RecordNumber = RecordNumber.Trim()
            };

            await _patientService.AddAsync(patient);
            MessageBox.Show("Pacijent uspešno dodat!", "Obaveštenje",
                MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadPatientsAsync();
            ClearForm();
        }

        private bool CanDeletePatient()
        {
            return SelectedPatient != null;
        }

        private async Task DeletePatientAsync()
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Molimo izaberite pacijenta!", "Upozorenje",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Da li ste sigurni da želite da obrišete pacijenta {SelectedPatient.FullName}?",
                "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await _patientService.DeleteAsync(SelectedPatient);
                MessageBox.Show("Pacijent obrisan!", "Obaveštenje",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadPatientsAsync();
            }
        }

        private void ClearForm()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            BirthDate = null;
            RecordNumber = string.Empty;
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}