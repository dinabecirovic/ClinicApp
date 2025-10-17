using ClinicApp.Services;
using ClinicApp.ViewModels;
using System.Windows;

namespace ClinicApp.Views
{
    public partial class AppointmentsWindow : Window
    {
        private readonly AppointmentsViewModel _viewModel;

        public AppointmentsWindow()
        {
            InitializeComponent();

            _viewModel = new AppointmentsViewModel(
                new AppointmentService(App.DbContext!),
                new PatientService(App.DbContext!),
                new StaffService(App.DbContext!)
            );

            _viewModel.DataChanged += (s, msg) =>
                MessageBox.Show(msg, "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);

            DataContext = _viewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadDataAsync();
        }
    }
}
