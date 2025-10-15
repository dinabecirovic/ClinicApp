using System.Windows;

namespace ClinicApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenPatients_Click(object sender, RoutedEventArgs e)
        {
            var window = new PatientsWindow();
            window.ShowDialog();
        }

        private void OpenAppointments_Click(object sender, RoutedEventArgs e)
        {
            var window = new AppointmentsWindow();
            window.ShowDialog();
        }

        private void OpenMedicalRecords_Click(object sender, RoutedEventArgs e)
        {
            var window = new MedicalRecordsWindow();
            window.ShowDialog();
        }

        private void OpenStaff_Click(object sender, RoutedEventArgs e)
        {
            var window = new StaffWindow();
            window.ShowDialog();
        }

        private void OpenAdministration_Click(object sender, RoutedEventArgs e)
        {
            var window = new AdministrationWindow();
            window.ShowDialog();
        }
    }
}