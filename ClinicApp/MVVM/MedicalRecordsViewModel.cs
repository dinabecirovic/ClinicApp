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
    public class MedicalRecordsViewModel : INotifyPropertyChanged
    {
        private readonly MedicalRecordService _medicalRecordService;
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
                _ = LoadRecordsAsync();
            }
        }

        private ObservableCollection<MedicalRecord> _medicalRecords = new();
        public ObservableCollection<MedicalRecord> MedicalRecords
        {
            get => _medicalRecords;
            set
            {
                _medicalRecords = value;
                OnPropertyChanged();
            }
        }

        private string _diagnosis = string.Empty;
        public string Diagnosis
        {
            get => _diagnosis;
            set
            {
                _diagnosis = value;
                OnPropertyChanged();
            }
        }

        private string _treatment = string.Empty;
        public string Treatment
        {
            get => _treatment;
            set
            {
                _treatment = value;
                OnPropertyChanged();
            }
        }

        private string _notes = string.Empty;
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand AddRecordCommand { get; }

        // Constructor
        public MedicalRecordsViewModel(MedicalRecordService medicalRecordService, PatientService patientService)
        {
            _medicalRecordService = medicalRecordService;
            _patientService = patientService;

            AddRecordCommand = new RelayCommand(AddRecordAsync, CanAddRecord);
        }

        // Methods
        public async Task LoadPatientsAsync()
        {
            var patients = await _patientService.GetAllAsync();
            Patients = new ObservableCollection<Patient>(patients);
        }

        private async Task LoadRecordsAsync()
        {
            if (SelectedPatient != null)
            {
                var records = await _medicalRecordService.GetByPatientIdAsync(SelectedPatient.Id);
                MedicalRecords = new ObservableCollection<MedicalRecord>(records);
            }
            else
            {
                MedicalRecords.Clear();
            }
        }

        private bool CanAddRecord()
        {
            return SelectedPatient != null && !string.IsNullOrWhiteSpace(Diagnosis);
        }

        private async Task AddRecordAsync()
        {
            if (SelectedPatient == null || string.IsNullOrWhiteSpace(Diagnosis))
            {
                MessageBox.Show("Pacijent i dijagnoza su obavezni!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var record = new MedicalRecord
            {
                PatientId = SelectedPatient.Id,
                Date = DateTime.Now,
                Diagnosis = Diagnosis.Trim(),
                Treatment = Treatment.Trim(),
                Notes = Notes.Trim()
            };

            await _medicalRecordService.AddAsync(record);
            MessageBox.Show("Medicinski karton dodat!", "Obaveštenje",
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Refresh records
            await LoadRecordsAsync();

            // Clear form
            ClearForm();
        }

        private void ClearForm()
        {
            Diagnosis = string.Empty;
            Treatment = string.Empty;
            Notes = string.Empty;
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // RelayCommand helper class
    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public async void Execute(object? parameter)
        {
            await _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}