using ClinicApp.Services;
using ClinicApp.ViewModels;
using System.Windows;

namespace ClinicApp.Views
{
    public partial class PatientsWindow : Window
    {
        private readonly PatientsViewModel _viewModel;

        public PatientsWindow()
        {
            InitializeComponent();

            // Inicijalizacija servisa
            var patientService = new PatientService(App.DbContext!);

            // Inicijalizacija ViewModel-a
            _viewModel = new PatientsViewModel(patientService);
            DataContext = _viewModel;

            // Učitavanje pacijenata
            Loaded += async (s, e) => await _viewModel.LoadPatientsAsync();
        }
    }
}