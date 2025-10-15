using ClinicApp.Delegates;
using ClinicApp.Models;
using ClinicApp.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ClinicApp.Views
{
    public partial class MedicalRecordsWindow : Window
    {
        private readonly MedicalRecordService _medicalRecordService;
        private readonly PatientService _patientService;
        public event DataChangedHandler? DataChanged;

        public MedicalRecordsWindow()
        {
            InitializeComponent();
            _medicalRecordService = new MedicalRecordService(App.DbContext!);
            _patientService = new PatientService(App.DbContext!);
            DataChanged += OnDataChanged;
        }

        private void OnDataChanged(object sender, string message)
        {
            MessageBox.Show(message, "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var patients = await _patientService.GetAllAsync();
            RecordPatientSelector.ItemsSource = patients;
        }

        private async void RecordPatient_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (RecordPatientSelector.SelectedItem is Patient selectedPatient)
            {
                var records = await _medicalRecordService.GetByPatientIdAsync(selectedPatient.Id);
                RecordsDataGrid.ItemsSource = records;
            }
        }

        private async void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            if (RecordPatientSelector.SelectedItem == null ||
                string.IsNullOrWhiteSpace(RecordDiagnosis.Text))
            {
                MessageBox.Show("Pacijent i dijagnoza su obavezni!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patient = (Patient)RecordPatientSelector.SelectedItem;

            var record = new MedicalRecord
            {
                PatientId = patient.Id,
                Date = DateTime.Now,
                Diagnosis = RecordDiagnosis.Text.Trim(),
                Treatment = RecordTreatment.Text.Trim(),
                Notes = RecordNotes.Text.Trim()
            };

            await _medicalRecordService.AddAsync(record);
            DataChanged?.Invoke(this, "Medicinski karton dodat!");

            // Refresh
            var records = await _medicalRecordService.GetByPatientIdAsync(patient.Id);
            RecordsDataGrid.ItemsSource = records;

            ClearForm();
        }

        private void ClearForm()
        {
            RecordDiagnosis.Clear();
            RecordTreatment.Clear();
            RecordNotes.Clear();
        }
    }
}