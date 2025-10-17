using ClinicApp.Services;
using ClinicApp.ViewModels;
using System.Windows;

namespace ClinicApp.Views
{
    public partial class MedicalRecordsWindow : Window
    {
        private readonly MedicalRecordsViewModel _viewModel;

        public MedicalRecordsWindow()
        {
            InitializeComponent();

            // Inicijalizacija servisa
            var medicalRecordService = new MedicalRecordService(App.DbContext!);
            var patientService = new PatientService(App.DbContext!);

            // Inicijalizacija ViewModel-a
            _viewModel = new MedicalRecordsViewModel(medicalRecordService, patientService);
            DataContext = _viewModel;

            // Učitavanje pacijenata
            Loaded += async (s, e) => await _viewModel.LoadPatientsAsync();
        }
    }
}